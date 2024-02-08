// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Plugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 1:50 AM
//    Created Date:     10/28/2023 1:50 AM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.GhostBusters.Configs;
using AutoEvent.Games.GhostBusters.Features;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using CustomPlayerEffects;
using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventoryMenu.API;
using InventoryMenu.API.EventArgs;
using InventoryMenu.API.Features;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.MicroHID;
using MapGeneration;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Doors;
using PluginAPI.Events;
using Powerups.Extensions;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.GhostBusters;

public class Plugin : Event, IInternalEvent, IEventSound, IHidden
{
    public override string Name { get; set; } = "Ghost Busters";
    public override string Description { get; set; } = "Ghostbusters vs ghosts. The clock is ticking, will the ghost-busters be able to kill all ghosts before midnight hits?";
    public override string Author { get; set; } = "Redforce04 and Riptide";
    public override string CommandName { get; set; } = "ghosts";
    public override Version Version { get; set; } = new Version(1, 0, 0);
    [EventConfig] 
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public SoundInfo SoundInfo { get; set; } = new SoundInfo()
    { 
        SoundName = "Ghostbusters.ogg", 
        Volume = 5, 
        Loop = true
    };
    protected override float PostRoundDelay { get; set; } = 10f;
    protected override float FrameDelayInSeconds { get; set; } = 1f;
    private EventHandler _eventHandler { get; set; }
    private TimeSpan _remainingTime;
    public  Dictionary<Player, GhostBusterClass> Classes; 
    public Loadouts Loadouts { get; set; }
    public List<Ability> Abilities { get; set; }
    internal Menu HunterRoleMenu { get; set; }
    internal Menu GhostRoleMenu { get; set; }
    internal Menu GhostPowerupMenu { get; set; }
    public List<Player> Hunters { get; set; }
    public Dictionary<ushort, float> HIDCache { get; set; }

    public Dictionary<ItemType, float> Damages = new Dictionary<ItemType, float>()
    {
        { ItemType.GrenadeHE, 600 },
        { ItemType.MicroHID, 10 },
        // { ItemType.SCP018, 600 },
        { ItemType.ParticleDisruptor, 75 },
        { ItemType.Jailbird, 50 },
    };
    public Stage CurrentStage { get; set; }
    public enum Stage { Prep, PreMidnight, Midnight }
    protected override void RegisterEvents()
    {
        Loadouts = new Loadouts(this);
        this.Classes = new Dictionary<Player, GhostBusterClass>();
        _eventHandler = new EventHandler(this);
        EventManager.RegisterEvents(_eventHandler);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
    }

    protected override void UnregisterEvents()
    {
        EventManager.UnregisterEvents(_eventHandler);
        Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
        Players.DropItem -= _eventHandler.OnDropItem;
        Players.DropAmmo -= _eventHandler.OnDropAmmo;
        Loadouts = null;
        _eventHandler = null;
    }
    protected override void OnStart()
    {
        HIDCache = new Dictionary<ushort, float>();
        Timing.CallDelayed(3f, () =>
        {
            List<RoomIdentifier> ids = RoomIdentifier.AllRoomIdentifiers.Where(x => x.ApiRoom.Identifier.Name is RoomName.HczCheckpointA or RoomName.HczCheckpointB).ToList();
            var doors = BreakableDoor.AllDoors.Where(x => x.Rooms.Any(room => ids.Contains(room)));
            DebugLogger.LogDebug($"Doors: {doors.Count()}");
            foreach (var x in PryableDoor.AllDoors.Where(X => X.Rooms.Any(rm => rm.Name == RoomName.HczCheckpointToEntranceZone && X is PryableDoor)))
            {
                if (x is not PryableDoor gate)
                {
                    continue;
                }
                gate.IsScp106Passable = false;
                    
                if(gate is IScp106PassableDoor door)
                    door.IsScp106Passable = false;

                /*if (gate is BreakableDoor breakableDoor)
                {
                    breakableDoor.IsScp106Passable = false;
                    breakableDoor.IgnoredDamageSources = DoorDamageType.Grenade | DoorDamageType.Scp096 |
                                                            DoorDamageType.Weapon | DoorDamageType.ServerCommand;  
                }*/
                    
                gate.ServerChangeLock(DoorLockReason.AdminCommand, true);
            }
        });
        CurrentStage = Stage.Prep;
        Abilities = new List<Ability>() { };
        var a = Config.Abilities[GhostBusterClassType.GhostInvisibility];
        Abilities.Add(new Ability(ItemType.SCP268, GhostBusterClassType.GhostInvisibility, "Invisibility - You can go invisible for a few seconds.", a.Cooldown, -2, (short) a.AllowedUses, a.Duration, Features.AbilityImplementations.UseInvisibilityAbility));
        a = Config.Abilities[GhostBusterClassType.GhostSpeed];
        Abilities.Add(new Ability(ItemType.SCP207, GhostBusterClassType.GhostSpeed, "Speed Rush - You can trigger a speed rush to get away.",  a.Cooldown, -2, (short)a.AllowedUses, a.Duration, Features.AbilityImplementations.UseSpeedEffect)); 
        a = Config.Abilities[GhostBusterClassType.GhostExplosive];
        Abilities.Add(new Ability(ItemType.GrenadeHE, GhostBusterClassType.GhostExplosive, "Explosive - You have a trap that can be placed.",  a.Cooldown, -2,(short) a.AllowedUses, a.Duration, Features.AbilityImplementations.UseExplosionEffect));
        a = Config.Abilities[GhostBusterClassType.GhostFlash];
        Abilities.Add(new Ability(ItemType.GrenadeFlash, GhostBusterClassType.GhostFlash, "Flash - You can flash the hunters for a brief period of time.", a.Cooldown, -2,(short) a.AllowedUses, a.Duration, Features.AbilityImplementations.UseFlashEffect));
        a = Config.Abilities[GhostBusterClassType.GhostBall];
        Abilities.Add(new Ability(ItemType.SCP018, GhostBusterClassType.GhostBall, "Ball - You have a ball that you can throw at hunters to do damage.", a.Cooldown, -2,(short) a.AllowedUses, a.Duration, Features.AbilityImplementations.UseBallEffect));
        a = Config.Abilities[GhostBusterClassType.GhostLockdown];
        Abilities.Add(new Ability(ItemType.SCP2176, GhostBusterClassType.GhostLockdown, "Lockdown - You can lock a room for a few moments to get away.", a.Cooldown, -2,(short) a.AllowedUses, a.Duration, Features.AbilityImplementations.UseLockdownEffect));
            
        Classes = new Dictionary<Player, GhostBusterClass>();
        Extensions.JailbirdIsInvincible = true;
        HunterRoleMenu = new Menu("Available Roles. Right click to view more details, left click to select the role.", false);
        HunterRoleMenu.AddItem(new MenuItem(ItemType.MicroHID, "Tank Loadout", 0, Loadouts.HuntersSelectLoadout));
        HunterRoleMenu.AddItem(new MenuItem(ItemType.ParticleDisruptor, "Sniper Loadout", 1, Loadouts.HuntersSelectLoadout));
        HunterRoleMenu.AddItem(new MenuItem(ItemType.Jailbird, "Melee Loadout", 2, Loadouts.HuntersSelectLoadout));
        GhostRoleMenu = new Menu("Available Roles. Right click to view more details, left click to select the role.", true);
        for (int i = 0; i < Abilities.Count; i++)
        {
            var ability = Abilities[i];
            GhostRoleMenu.AddItem(new MenuItem(ability.ItemType, ability.Description, (byte)i, Loadouts.GhostSelectLoadout, null, ability.ItemBase));
        }
            
        GhostPowerupMenu = new Menu("Powerup Menu", true, Loadouts.ProcessGetMenuArgs);
        for (int i = 0; i < Abilities.Count; i++)
        {
            var ability = Abilities[i];
            GhostPowerupMenu.AddItem(new MenuItem(ability.ItemType, ability.Description, (byte)i, Loadouts.GhostUseAbility, null, ability.ItemBase));
        }
            
        _remainingTime = new TimeSpan(0,0,Config.TimeUntilMidnightInSeconds);
        Hunters = Config.HunterCount.GetPlayers(true);
            
        foreach(Player ply in Player.GetPlayers())
        {
            if (Hunters.Contains(ply))
                SetHunter(ply);
            else
                SetGhost(ply);
        }
            
    }

    private void SetHunter(Player ply)
    {
        Classes.Add(ply, new GhostBusterClass(ply, GhostBusterClassType.HunterUnchosen));
        ply.ClearBroadcasts();
        ply.SendBroadcast(Translation.StartHunterMessage, 15);
        ply.SetRole(RoleTypeId.Scp049, RoleChangeReason.Respawn, RoleSpawnFlags.UseSpawnpoint);
        Timing.CallDelayed(0.25f, () =>
        {
            ply.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Respawn, RoleSpawnFlags.None);
        });
        ply.ShowMenu(HunterRoleMenu);
    }

    private void SetGhost(Player ply)
    {
        Classes.Add(ply, new GhostBusterClass(ply, GhostBusterClassType.GhostUnchosen));
        ply.ClearBroadcasts();
        ply.SendBroadcast(Translation.StartGhostMessage, 15);
        ply.SetRole(RoleTypeId.ClassD, RoleChangeReason.Respawn, RoleSpawnFlags.UseSpawnpoint);
        ply.SetRole(RoleTypeId.Scientist, RoleChangeReason.Respawn, RoleSpawnFlags.None);
        ply.GiveLoadout(Config.GhostLoadouts);
        ply.ShowMenu(GhostRoleMenu);
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (float _time = 15; _time > 0; _time--)
        {
            //Extensions.Broadcast(Translation.Replace("{time}", $"{_time}"), 1);

            yield return Timing.WaitForSeconds(1f);
            EventTime += TimeSpan.FromSeconds(1f);
        }

            
    }

    protected override void CountdownFinished()
    {
        _remainingTime = TimeSpan.FromSeconds(Config.TimeUntilMidnightInSeconds);
        CurrentStage = Stage.PreMidnight;
    }

    protected override bool IsRoundDone()
    {
        int ghosts = Player.GetPlayers().Count(x => this.IsGhost(x));
        int hunters = Player.GetPlayers().Count(x => this.IsHunter(x));
        return !(hunters >= 1 && ghosts >= 1 && this.EventTime.TotalSeconds < Config.TimeUntilMidnightInSeconds + Config.MidnightDurationInSeconds);
    }

    private void StartMidnight()
    {
        foreach (Player ply in Player.GetPlayers())
        {
            ply.HideMenu();
            if (ply.IsAlive && this.IsHunter(ply))
            {
                continue;
            }
            if (!ply.IsAlive)
            {
                ply.SetRole(RoleTypeId.Scp0492, RoleChangeReason.Respawn, RoleSpawnFlags.UseSpawnpoint);
                ply.Position = Features.TPUtils.GetRoomInHeavy(ply);
            }
            else
            {
                RoleTypeId role = (UnityEngine.Random.Range(0, 6)) switch
                {
                    0 => RoleTypeId.Scp049,
                    1 => RoleTypeId.Scp173,
                    2 => RoleTypeId.Scp106,
                    3 => RoleTypeId.Scp096,
                    4 => RoleTypeId.Scp939,
                    _ => RoleTypeId.Scp0492
                };
                ply.SetRole(role, RoleChangeReason.Escaped, RoleSpawnFlags.AssignInventory);
            }
            ply.Health = 2000;
        }
    }

        
    protected override void ProcessFrame()
    {
        if (_remainingTime.TotalSeconds <= 0)
        {
            if (CurrentStage == Stage.PreMidnight)
            {
                CurrentStage = Stage.Midnight;
                _remainingTime = TimeSpan.FromSeconds(Config.MidnightDurationInSeconds);
                StartMidnight();
            }
            else
            {
                DebugLogger.LogDebug($"Game should be over. {EventTime.TotalSeconds}, {Config.MidnightDurationInSeconds} + {Config.TimeUntilMidnightInSeconds}");
                return;
            }
        }

        var time = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";
        int ghosts = Player.GetPlayers().Count(ply => this.IsGhost(ply));
        int hunters = Player.GetPlayers().Count(ply => this.IsHunter(ply));

        foreach (Player player in Player.GetPlayers())
        {
            List<ushort> itemIds = player.ReferenceHub.inventory.UserInventory.Items.Keys.ToList();
            for (int i = 0; i < player.Items.Count; i++)
            {
                var item = player.ReferenceHub.inventory.UserInventory.Items[itemIds[i]];
                if (item is not InventorySystem.Items.MicroHID.MicroHIDItem microHidItem)
                    continue;
                if (microHidItem.EnergyToByte == byte.MaxValue)
                    continue;
                    
                if(!HIDCache.ContainsKey(microHidItem.ItemSerial))
                    HIDCache.Add(microHidItem.ItemSerial, 0);

                if (HIDCache[microHidItem.ItemSerial] >= Config.MicroRechargeDelayOffset)
                {
                    microHidItem.RemainingEnergy += Config.MicroRechargePercentPerSecond * .01f;
                }
                if (microHidItem.State is not HidState.Idle)
                    HIDCache[microHidItem.ItemSerial] = 0f;
                else
                    HIDCache[microHidItem.ItemSerial] += this.FrameDelayInSeconds;
                // DebugLogger.LogDebug($"Micro {microHidItem.ItemSerial}, Delay: [{HIDCache[microHidItem.ItemSerial]}], Energy: {microHidItem.RemainingEnergy} [{microHidItem.EnergyToByte}]");
                // player.ReferenceHub.inventory.UserInventory.Items[itemIds[i]] = microHidItem;
            }
            if (Classes.ContainsKey(player) && Classes[player].AbilityCooldown > 0)
            {
                Classes[player].AbilityCooldown = Mathf.Clamp(Classes[player].AbilityCooldown - FrameDelayInSeconds, 0, 10000);
            }
            if (CurrentStage == Stage.Midnight)
            {

                if (this.IsGhost(player))
                {
                    player.SendBroadcast(Translation.MidnightGhostMessage.Replace("{time}", time), (ushort) FrameDelayInSeconds);
                }
                else
                {
                    player.SendBroadcast(Translation.MidnightHunterMessage.Replace("{time}", time), (ushort) FrameDelayInSeconds);
                }
            }
            else
            {
                player.SendBroadcast(
                    Translation.Running.Replace("{time}", time).Replace("{ghosts}", ghosts.ToString())
                        .Replace("{hunters}", hunters.ToString()), (ushort)FrameDelayInSeconds);
            }
        }

        //var a = PluginAPI.Core.Map.Rooms.First(x => x.Name == RoomName.HczCheckpointA);
            //var b = PluginAPI.Core.Map.Rooms.First(x => x.Name == RoomName.HczCheckpointB);
            //a.ApiRoom
            //var nameTag = a.gameObject.GetComponentInChildren<DoorNametagExtension>().TargetDoor.ServerChangeLock() ? name.GetName : null;

            _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
    }

    protected override void OnFinished()
    {
        int hunters = Player.GetPlayers().Count(x => IsHunter(x));
        int ghosts = Player.GetPlayers().Count(x => IsGhost(x));
        if (hunters > 0)
        {
            if(ghosts > 0)
                Map.Broadcast(10, Translation.Tie);
            else
                Map.Broadcast(10, Translation.HuntersWin);
        }
        else
        {
            Map.Broadcast(10, Translation.GhostsWin);
        }
    }

    protected override void OnCleanup()
    {
        foreach (var x in PryableDoor.AllDoors.Where(X => X.Rooms.Any(rm => rm.Name == RoomName.HczCheckpointToEntranceZone && X is PryableDoor)))
        {
            if (x is not PryableDoor gate)
            {
                continue;
            }
                
                
            gate.IsScp106Passable = true;
                    
            if(gate is IScp106PassableDoor door)
                door.IsScp106Passable = true;

            /*if (gate is BreakableDoor breakableDoor)
            {
                breakableDoor.IsScp106Passable = true;
                breakableDoor.IgnoredDamageSources = DoorDamageType.None;      
            }*/
            gate.ServerChangeLock(DoorLockReason.AdminCommand, false);
        }
        base.OnCleanup();
        foreach (Player ply in Player.GetPlayers())
        {
            ply.HideMenu();
        }
    }

    public bool IsGhost(Player ply)
    {
        return ply.IsAlive && !IsHunter(ply);
    }
    public bool IsHunter(Player ply)
    {
        return this.Hunters.Contains(ply);
    }
}