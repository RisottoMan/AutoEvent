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
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using CustomPlayerEffects;
using Interactables.Interobjects.DoorUtils;
using InventoryMenu.API;
using InventoryMenu.API.EventArgs;
using InventoryMenu.API.Features;
using MapGeneration;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using Powerups.Extensions;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.GhostBusters;

public class Plugin : Event, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate.GhostBustersName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate.GhostBustersDescription;
        public override string Author { get; set; } = "Redforce04 and Riptide";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate.GhostBustersCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig] public GhostBustersConfig Config { get; set; }
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Ghostbusters.ogg", Volume = 5, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        protected override float FrameDelayInSeconds { get; set; } = 1f;
        private EventHandler EventHandler { get; set; }
        private GhostBustersTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate;
        private TimeSpan _remainingTime;
        internal Menu HunterRoleMenu { get; set; }
        internal Menu GhostRoleMenu { get; set; }
        internal Menu GhostPowerupMenu { get; set; }
        
        public enum Stage { Prep, PreMidnight, Midnight }
        protected override void RegisterEvents()
        {

            EventHandler = new EventHandler(this);
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropItem -= EventHandler.OnDropItem;
            Players.DropAmmo -= EventHandler.OnDropAmmo;

            EventHandler = null;
        }
        protected override void OnStart()
        {
            Extensions.JailbirdIsInvincible = true;
            HunterRoleMenu = new Menu("Available Roles. Right click to view more details, left click to select the role.", false);
            HunterRoleMenu.AddItem(new MenuItem(ItemType.MicroHID, "Tank Loadout", 0, HuntersSelectLoadout));
            HunterRoleMenu.AddItem(new MenuItem(ItemType.ParticleDisruptor, "Sniper Loadout", 1, HuntersSelectLoadout));
            HunterRoleMenu.AddItem(new MenuItem(ItemType.Jailbird, "Melee Loadout", 2, HuntersSelectLoadout));
            GhostRoleMenu = new Menu("Available Roles. Right click to view more details, left click to select the role.", true);
            GhostPowerupMenu = new Menu("Powerup Menu", true);
            GhostPowerupMenu.AddItem(new MenuItem(ItemType.Medkit, "Heal", 0));
            GhostPowerupMenu.AddItem(new MenuItem(ItemType.SCP268, "", 0));
            GhostPowerupMenu.AddItem(new MenuItem(ItemType.SCP018, "", 0));
            _remainingTime = new TimeSpan(0,0,Config.TimeUntilMidnightInSeconds);
            var hunters = Config.HunterCount.GetPlayers(true);
            foreach(Player ply in Player.GetPlayers())
            {
                if (hunters.Contains(ply))
                    SetHunter(ply);
                else
                    SetGhost(ply);
            }
            
        }

        public void ProcessGetMenuArgs(GetMenuItemsForPlayerArgs ev)
        {
            
        }

        private void HuntersSelectLoadout(MenuItemClickedArgs ev)
        {
            switch (ev.MenuItemClicked.Item)
            {
                case ItemType.MicroHID:
                    ev.Player.GiveLoadout(Config.TankLoadout);
                    break;
                case ItemType.ParticleDisruptor:
                    ev.Player.GiveLoadout(Config.SniperLoadout);
                    break;
                case ItemType.Jailbird:
                    ev.Player.GiveLoadout(Config.MeleeLoadout);
                    if(ev.Player.EffectsManager.GetEffect<MovementBoost>()?.Intensity > 0)
                        ev.Player.ApplyFakeEffect<Scp207>(1);
                    break;
            }
            ev.Player.HideMenu();
            
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.CurrentItem = ev.Player.Items.First(x => x.ItemTypeId.IsWeapon());
            });
            
        }
        
        private void SetHunter(Player ply)
        {
            ply.ClearBroadcasts();
            ply.SendBroadcast(Translation.GhostBustersStartHunterMessage, 15);
            ply.SetRole(RoleTypeId.Scp049, RoleChangeReason.Respawn, RoleSpawnFlags.UseSpawnpoint);
            Timing.CallDelayed(0.25f, () =>
            {
                ply.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Respawn, RoleSpawnFlags.None);
            });
            ply.ShowMenu(HunterRoleMenu);
            
        }

        private void SetGhost(Player ply)
        {
            ply.ClearBroadcasts();
            ply.SendBroadcast(Translation.GhostBustersStartGhostMessage, 15);
            ply.GiveLoadout(Config.GhostLoadouts);
            ply.ShowMenu(GhostPowerupMenu);
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

        protected override bool IsRoundDone()
        {
            return true;
        }

        protected override void ProcessFrame()
        {
            var time = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";
                foreach (Player player in Player.GetPlayers())
                {
                    //player.SendBroadcast("");
                }

                //var a = PluginAPI.Core.Map.Rooms.First(x => x.Name == RoomName.HczCheckpointA);
                //var b = PluginAPI.Core.Map.Rooms.First(x => x.Name == RoomName.HczCheckpointB);
                //a.ApiRoom
                //var nameTag = a.gameObject.GetComponentInChildren<DoorNametagExtension>().TargetDoor.ServerChangeLock() ? name.GetName : null;

                _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        }

        protected override void OnFinished()
        {
            int ghosts = Player.GetPlayers().Count(x => !x.HasLoadout(Config.MeleeLoadout) && !x.HasLoadout(Config.SniperLoadout) &&
                                                        !x.HasLoadout(Config.TankLoadout));
            if (ghosts > 0)
            {
                Map.Broadcast(10, Translation.GhostBustersGhostsWin);
            }
            else
            {
                Map.Broadcast(10, Translation.GhostBustersHuntersWin);
            }
        }
    }