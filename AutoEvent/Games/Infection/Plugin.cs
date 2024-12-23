using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using InventorySystem.Items.MarshmallowMan;
using UnityEngine;

namespace AutoEvent.Games.Infection;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Zombie Infection";
    public override string Description { get; set; } = "Zombie mode, the purpose of which is to infect all players";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "zombie";
    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "Zombie", 
        Position = new Vector3(115.5f, 1030f, -43.5f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "Zombie_Run.ogg", 
        Volume = 15
    };
    private EventHandler _eventHandler { get; set; }
    private int _overtime = 30;
    internal List<GameObject> SpawnList;
    public bool IsChristmasUpdate { get; set; } = false;
    public bool IsHalloweenUpdate { get; set; } = false;

    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurting;
        Exiled.Events.Handlers.Player.Joined += _eventHandler.OnJoined;
        Exiled.Events.Handlers.Player.Died += _eventHandler.OnDied;
    }

    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurting;
        Exiled.Events.Handlers.Player.Joined -= _eventHandler.OnJoined;
        Exiled.Events.Handlers.Player.Died -= _eventHandler.OnDied;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _overtime = 30;
        // Halloween update
        if (Enum.IsDefined(typeof(ItemType), "Marshmallow"))
        {
            IsHalloweenUpdate = true;
            ForceEnableFriendlyFire = FriendlyFireSettings.Enable;
        }
        // Christmas update
        else if (Enum.IsDefined(typeof(RoleTypeId), "ZombieFlamingo"))
        {
            IsChristmasUpdate = true;
        }

        SpawnList = MapInfo.Map.AttachedBlocks.Where(r => r.name == "Spawnpoint").ToList();
        foreach (Player player in Player.List)
        {
            if (IsChristmasUpdate)
            {
                RoleTypeId roleType = (RoleTypeId)Enum.Parse(typeof(RoleTypeId), "Flamingo");
                player.Role.Set(roleType, RoleSpawnFlags.None);
            }
            else
            {
                player.GiveLoadout(Config.PlayerLoadouts);
            }

            player.Position = SpawnList.RandomItem().transform.position;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (float time = 15; time > 0; time--)
        {
            Extensions.Broadcast(Translation.Start.
                Replace("{name}", Name).
                Replace("{time}", time.ToString("00")), 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override void CountdownFinished()
    {
        Player player = Player.List.ToList().RandomItem();
        
        if (IsHalloweenUpdate)
        {
            player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
            player.EnableEffect<MarshmallowEffect>();
            player.IsGodModeEnabled = true;
        }
        else if (IsChristmasUpdate)
        {
            RoleTypeId roleType = (RoleTypeId)Enum.Parse(typeof(RoleTypeId), "Flamingo");
            player.Role.Set(roleType, RoleSpawnFlags.None);
        }
        else
        {
            player.GiveLoadout(Config.ZombieLoadouts);
        }
        
        //Extensions.PlayPlayerAudio(player, Config.ZombieScreams.RandomItem(), 15);
    }

    protected override bool IsRoundDone()
    {
        RoleTypeId roleType = RoleTypeId.ClassD;
        if (IsChristmasUpdate)
        {
            roleType = (RoleTypeId)Enum.Parse(typeof(RoleTypeId), "Flamingo");
        }

        if (Player.List.Count(r => r.Role == roleType) > 0 && _overtime > 0)
        {
            return false;
        }
        
        return true;
    }
    
    protected override void ProcessFrame()
    {
        RoleTypeId roleType = RoleTypeId.ClassD;
        string time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
        
        if (IsChristmasUpdate)
        {
            roleType = (RoleTypeId)Enum.Parse(typeof(RoleTypeId), "Flamingo");
        }
        
        int count = Player.List.Count(r => r.Role == roleType);

        if (count > 1)
        {
            Extensions.Broadcast(Translation.Cycle.
                Replace("{name}", Name).
                Replace("{count}", count.ToString()).
                Replace("{time}", time), 1);
        }
        else if (count == 1)
        {
            _overtime--;
            Extensions.Broadcast(Translation.ExtraTime
                    .Replace("{extratime}", _overtime.ToString("00"))
                    .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 1);
        }
    }

    protected override void OnFinished()
    {
        RoleTypeId roleType = RoleTypeId.ClassD;
        if (IsChristmasUpdate)
        {
            roleType = (RoleTypeId)Enum.Parse(typeof(RoleTypeId), "Flamingo");
        }
        
        if (Player.List.Count(r => r.Role == roleType) == 0)
        {
            Extensions.Broadcast(Translation.Win.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
        }
        else
        {
            Extensions.Broadcast(Translation.Lose.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
        }
    }
}