using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Games.Line;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Death Line";
    public override string Description { get; set; } = "Avoid the spinning platform to survive";
    public override string Author { get; set; } = "Logic_Gun & RisottoMan";
    public override string CommandName { get; set; } = "line";
    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "Line", 
        Position = new Vector3(0f, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "LineLite.ogg", 
        Volume = 10
    };
    private TimeSpan _timeRemaining;
    protected override void OnStart()
    {
        _timeRemaining = new TimeSpan(0, 2, 0);
        
        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.Loadouts);
            player.Position = MapInfo.Map.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast($"{time}", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override void CountdownFinished()
    {
        foreach (var block in MapInfo.Map.AttachedBlocks)
        {
            switch (block.name)
            {
                case "DeadZone": block.AddComponent<LineComponent>().Init(this, ObstacleType.MiniWalls); break;
                case "DeadWall": block.AddComponent<LineComponent>().Init(this, ObstacleType.Wall); break;
                case "Line": block.AddComponent<LineComponent>().Init(this, ObstacleType.Ground); break;
                case "Shield": GameObject.Destroy(block); break;
            }
        }
    }

    protected override void ProcessFrame()
    {
        Extensions.Broadcast(Translation.Cycle.Replace("{name}", Name).
            Replace("{time}", $"{_timeRemaining.Minutes:00}:{_timeRemaining.Seconds:00}").
            Replace("{count}", $"{Player.List.Count(r => r.HasLoadout(Config.Loadouts))}"), 10);
        
        _timeRemaining -= TimeSpan.FromSeconds(FrameDelayInSeconds);
    }

    protected override bool IsRoundDone()
    {
        // At least 2 players &&
        // Time is smaller than 2 minutes (+countdown)
        return !(Player.List.Count(r => r.HasLoadout(Config.Loadouts)) > 1 && EventTime.TotalSeconds < 120);
    }

    protected override void OnFinished()
    {
        if (Player.List.Count(r => r.Role != AutoEvent.Singleton.Config.LobbyRole) > 1)
        {
            Extensions.Broadcast(Translation.MorePlayers.
                Replace("{name}", Name).
                Replace("{count}", $"{Player.List.Count(r => r.HasLoadout(Config.Loadouts))}"), 10);
        }
        else if (Player.List.Count(r => r.Role != AutoEvent.Singleton.Config.LobbyRole) == 1)
        {
            Extensions.Broadcast(Translation.Winner.
                Replace("{name}", Name).
                Replace("{winner}", Player.List.First(r => r.HasLoadout(Config.Loadouts)).Nickname), 10);
        }
        else
        {
            Extensions.Broadcast(Translation.AllDied, 10);
        }
    }
}