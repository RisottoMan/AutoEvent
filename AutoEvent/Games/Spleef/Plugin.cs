using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace AutoEvent.Games.Spleef;

public class Plugin : Event<Config, Translation>, IEventMap
{
    public override string Name { get; set; } = "Spleef";
    public override string Description { get; set; } = "Shoot at the platforms and don't fall into the void";
    public override string Author { get; set; } = "Redforce04 (created logic code) && RisottoMan (modified map)";
    public override string CommandName { get; set; } = "spleef";
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "Spleef",
        Position = new Vector3(0f, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    {
        SoundName = "Fall_Guys_Winter_Fallympics.ogg",
        Volume = 7
    };

    private EventHandler _eventHandler;
    private TimeSpan _countdown;
    private List<Loadout> _loadouts;
    
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Shot += _eventHandler.OnShot;
    }

    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Shot -= _eventHandler.OnShot;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _countdown = TimeSpan.FromSeconds(Config.RoundDurationInSeconds);
        _loadouts = new List<Loadout>();
        GameObject spawnpoint = new GameObject();

        GameObject lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
        lava.AddComponent<LavaComponent>().StartComponent(this);

        foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
        {
            switch(gameObject.name)
            {
                case "Spawnpoint": spawnpoint = gameObject; break;
                case "Platform": gameObject.AddComponent<FallPlatformComponent>(); break; //todo
            }
        }
        
        int count = Player.List.Count();
        switch (count)
        {
            case <= 5: _loadouts = Config.PlayerLittleLoadouts; break;
            case >= 15: _loadouts = Config.PlayerBigLoadouts; break;
            default: _loadouts = Config.PlayerNormalLoadouts; break;
        }

        foreach (Player ply in Player.List)
        {
            ply.GiveLoadout(_loadouts, LoadoutFlags.IgnoreWeapons);
            ply.Position = spawnpoint.transform.position;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast($"{Translation.Start.Replace("{time}", $"{time}")}", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }
    
    protected override void CountdownFinished()
    {
        foreach (Player ply in Player.List)
        {
            ply.GiveLoadout(_loadouts, LoadoutFlags.ItemsOnly);
        }
    }

    protected override bool IsRoundDone()
    {
        _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
        return !(Player.List.Count(ply => ply.IsAlive) > 1 && _countdown != TimeSpan.Zero);
    }

    protected override void ProcessFrame()
    {
        Extensions.Broadcast(Translation.Cycle.
                Replace("{name}", Name).
                Replace("{players}", $"{Player.List.Count(x => x.IsAlive)}").
                Replace("{remaining}", $"{_countdown.Minutes:00}:{_countdown.Seconds:00}"), 1);
    }

    protected override void OnFinished()
    {
        string text;
        int count = Player.List.Count(x => x.IsAlive);

        if (count > 1)
        {
            text = Translation.SomeSurvived;
        }
        else if (count == 1)
        {
            text = Translation.Winner.Replace("{winner}", Player.List.First(x => x.IsAlive).Nickname);
        }
        else
        {
            text = Translation.AllDied;
        }

        Extensions.Broadcast(text, 10);
    }
}