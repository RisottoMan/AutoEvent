using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Spleef;

public class Plugin : Event, IEventMap, IInternalEvent
{
    public override string Name { get; set; } = "Spleef";
    public override string Description { get; set; } = "Shoot at the platforms and don't fall into the void";
    public override string Author { get; set; } = "Redforce04 (created logic code) && KoT0XleB (modified map)";
    public override string CommandName { get; set; } = "spleef";
    public override Version Version { get; set; } = new Version(1, 0, 5);
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
    [EventConfig]
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public EventHandler _eventHandler { get; set; }
    public MapInfo MapInfo { get; set; } = new MapInfo()
    { 
        MapName = "Spleef",
        Position = new Vector3(76f, 1026.5f, -43.68f)
    };
    public SoundInfo SoundInfo { get; set; } = new SoundInfo()
    {
        SoundName = "Fall_Guys_Winter_Fallympics.ogg",
        Volume = 7
    };
    private TimeSpan _countdown;
    private List<GameObject> _platforms;
    List<Loadout> _loadouts;
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
        Players.Shot += _eventHandler.OnShot;
        EventManager.RegisterEvents(_eventHandler);
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
        Players.Shot -= _eventHandler.OnShot;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _countdown = TimeSpan.FromSeconds(Config.RoundDurationInSeconds);
        _platforms = new();
        _loadouts = new List<Loadout>();

        GameObject lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
        lava.AddComponent<LavaComponent>().StartComponent(this);
        _platforms = Methods.GeneratePlatforms(this);

        int count = Player.GetPlayers().Count();
        switch (count)
        {
            case <= 5: _loadouts = Config.PlayerLittleLoadouts; break;
            case >= 15: _loadouts = Config.PlayerBigLoadouts; break;
            default: _loadouts = Config.PlayerNormalLoadouts; break;
        }

        foreach (Player ply in Player.GetPlayers())
        {
            ply.GiveLoadout(_loadouts, LoadoutFlags.IgnoreWeapons);
            ply.Position = MapInfo.Position + new Vector3(0, Config.LayerCount * 3f + 5, 0);
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast($"{Translation.Description}\n{Translation.Start.Replace("{time}", $"{time}")}", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }
    
    protected override void CountdownFinished()
    {
        foreach (Player ply in Player.GetPlayers())
        {
            ply.GiveLoadout(_loadouts, LoadoutFlags.ItemsOnly);
        }
    }

    protected override bool IsRoundDone()
    {
        _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
        return !(Player.GetPlayers().Count(ply => ply.IsAlive) > 1) &&
             EventTime.TotalSeconds < Config.RoundDurationInSeconds;
    }

    protected override void ProcessFrame()
    {
        Extensions.Broadcast(Translation.Cycle.
                Replace("{name}", Name).
                Replace("{players}", $"{Player.GetPlayers().Count(x => x.IsAlive)}").
                Replace("{remaining}", $"{_countdown.Minutes:00}:{_countdown.Seconds:00}"), 1);
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        int count = Player.GetPlayers().Count(x => x.IsAlive);

        if (count > 1)
        {
            text = Translation.SomeSurvived;
        }
        else if (count == 1)
        {
            text = Translation.Winner.Replace("{winner}", Player.GetPlayers().First(x => x.IsAlive).Nickname);
        }
        else
        {
            text = Translation.AllDied;
        }

        Extensions.Broadcast(text, 10);
    }

    protected override void OnCleanup()
    {
        foreach (var platform in _platforms)
        {
            GameObject.Destroy(platform);
        }
    }
}