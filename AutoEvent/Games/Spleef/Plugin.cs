using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Spleef.Configs;
using AutoEvent.Games.Spleef.Features;
using AutoEvent.Interfaces;
using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Spleef;

public class Plugin : Event, IEventMap, IInternalEvent, IEventTag
{
    public override string Name { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate.SpleefName;
    public override string Description { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate.SpleefDescription;
    public override string Author { get; set; } = "Redforce04 (created logic code) && KoT0XleB (rewrite code and map)";
    public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate.SpleefCommandName;
    public override Version Version { get; set; } = new Version(1, 0, 2);
    public SpleefTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.SpleefTranslate;
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;

    [EventConfig]
    public SpleefConfig Config { get; set; }
    public EventHandler EventHandler { get; set; }
    public MapInfo MapInfo { get; set; } = new MapInfo()
    { 
        MapName = "Spleef", 
        Position = new Vector3(0, 0, 30f)
    };
    public SoundInfo SoundInfo { get; set; } = new SoundInfo()
    {
        SoundName = "Puzzle.ogg",
        Volume = 15,
        Loop = true
    };
    public TagInfo TagInfo { get; set; } = new TagInfo()
    {
        Name = "New Map",
        Color = "#77dde7"
    };
    TimeSpan RoundTime { get; set; }
    GameObject Wall { get; set; }
    List<API.Loadout> PlayerLoadounts { get; set; }
    protected override void RegisterEvents()
    {
        EventHandler = new EventHandler(this);

        Servers.TeamRespawn += EventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += EventHandler.OnPlaceBullet;
        Servers.PlaceBlood += EventHandler.OnPlaceBlood;
        Players.DropItem += EventHandler.OnDropItem;
        Players.DropAmmo += EventHandler.OnDropAmmo;
        Players.Shot += EventHandler.OnShot;
        EventManager.RegisterEvents(EventHandler);
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
        Players.Shot -= EventHandler.OnShot;

        EventHandler = null;
    }

    protected override void OnStart()
    {
        RoundTime = new TimeSpan(0, 0, Config.RoundDurationInSeconds);

        GameObject spawnpoint = new GameObject();
        foreach(GameObject gameObject in MapInfo.Map.AttachedBlocks)
        {
            switch (gameObject.name)
            {
                case "Lava": gameObject.AddComponent<LavaComponent>(); break;
                case "Platform": gameObject.AddComponent<FallPlatformComponent>(); break;
                case "Wall": Wall = gameObject; break;
                case "Spawnpoint": spawnpoint = gameObject; break;
            }
        }

        int plyCount = Player.GetPlayers().Count();
        if (plyCount <= 5)
        {
            PlayerLoadounts = Config.PlayerLittleLoadouts;
        }
        else if (plyCount >= 15)
        {
            PlayerLoadounts = Config.PlayerBigLoadouts;
        }
        else
        {
            PlayerLoadounts = Config.PlayerNormalLoadouts;
        }

        foreach (Player ply in Player.GetPlayers())
        {
            ply.GiveLoadout(PlayerLoadounts, LoadoutFlags.IgnoreWeapons);
            ply.Position = spawnpoint.transform.position;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 15; time > 0; time--)
        {
            Extensions.Broadcast($"{Translation.SpleefDescription}\n" +
                $"{Translation.SpleefStart.Replace("{time}", $"{time}")}", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }
    
    protected override void CountdownFinished()
    {
        foreach (Player ply in Player.GetPlayers())
        {
            ply.GiveLoadout(PlayerLoadounts, LoadoutFlags.ItemsOnly);
        }

        GameObject.Destroy(Wall);
    }

    protected override bool IsRoundDone()
    {
        RoundTime -= TimeSpan.FromSeconds(1f);
        return !(Player.GetPlayers().Count(ply => ply.IsAlive) > 1) &&
            RoundTime.TotalSeconds > 0;
    }

    protected override void ProcessFrame()
    {
        var remaining = $"{RoundTime.Minutes:00}:{RoundTime.Seconds:00}";
        int count = Player.GetPlayers().Count(x => x.IsAlive);
        foreach (Player ply in Player.GetPlayers())
        {
            ply.SendBroadcast(Translation.SpleefRunning.
                Replace("{players}", count.ToString()).
                Replace("{remaining}", $"{remaining}"), (ushort)this.FrameDelayInSeconds);
        }
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        int count = Player.GetPlayers().Count(x => x.IsAlive);

        if (count > 1)
        {
            text = Translation.SpleefSeveralSurvivors;
        }
        else if (count == 1)
        {
            text = Translation.SpleefWinner.
                Replace("{winner}", Player.GetPlayers().First(x => x.IsAlive).Nickname);
        }
        else
        {
            text = Translation.SpleefAllDied;
        }

        Server.SendBroadcast(text, 10);
    }
}