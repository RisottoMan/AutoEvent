using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using CustomPlayerEffects;
using PlayerRoles;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Deathrun;
public class Plugin : Event, IEventMap, IInternalEvent
{
    public override string Name { get; set; } = "Death Run";
    public override string Description { get; set; } = "Go to the end, avoiding death-activated trap along the way";
    public override string Author { get; set; } = "RisottoMan/code & xleb.ik/map";
    public override string CommandName { get; set; } = "deathrun";
    public override Version Version { get; set; } = new Version(1, 0, 2);
    [EventConfig]
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "TempleMap", 
        Position = new Vector3(0, 30, 30)
    };
    private EventHandler _eventHandler { get; set; }
    private GameObject _wall { get; set; }
    private List<GameObject> runnerSpawns { get; set; }
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler();
        EventManager.RegisterEvents(_eventHandler);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
    }

    protected override void UnregisterEvents()
    {
        EventManager.UnregisterEvents(_eventHandler);
        Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
        Players.DropItem -= _eventHandler.OnDropItem;
        Players.DropAmmo -= _eventHandler.OnDropAmmo;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        runnerSpawns = new();
        List<GameObject> deathSpawns = new();
        foreach (var block in MapInfo.Map.AttachedBlocks)
        {
            switch (block.name)
            {
                case "Spawnpoint": runnerSpawns.Add(block); break;
                case "Spawnpoint1": deathSpawns.Add(block); break;
                case "Wall": _wall = block; break;
            }
        }
        
        // Making a random death-guy and teleport to spawnpoint
        for (int i = 0; Player.GetPlayers().Count() / 20 >= i; i++)
        {
            Player death = Player.GetPlayers().Where(r => r.Role != RoleTypeId.Scientist).ToList().RandomItem();
            death.GiveLoadout(Config.DeathLoadouts);
            death.Position = deathSpawns.RandomItem().transform.position;
        }
        
        // Teleport runners to spawnpoint
        foreach (Player runner in Player.GetPlayers().Where(r => r.Role != RoleTypeId.Scientist))
        {
            runner.GiveLoadout(Config.PlayerLoadouts);
            runner.Position = runnerSpawns.RandomItem().transform.position;
        }
    }

    // Counting down the time to the start of the game
    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (float time = 10; time > 0; time--)
        {
            string text = Translation.BeforeStartBroadcast.
                Replace("{name}", Name).
                Replace("{time}", $"{time}");
            Extensions.Broadcast(text, 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }
    
    // Destroy the wall so that players can start passing the map
    protected override void CountdownFinished()
    {
        _wall.transform.position += new Vector3(0, 10, 0);
    }

    // While all the players are alive and time has not over
    protected override bool IsRoundDone()
    {
        return !(Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) > 0 &&
                 Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0 &&
                 EventTime.TotalSeconds < Config.RoundDurationInSeconds);
    }

    // All the logic of the game is handled by AMERT traps, so there is nothing here except the broadcast
    protected override void ProcessFrame()
    {
        TimeSpan timeleft = TimeSpan.FromSeconds(Config.RoundDurationInSeconds - EventTime.TotalSeconds);
        string timetext = $"{timeleft.Minutes:00}:{timeleft.Seconds:00}";
        
        if (timeleft.TotalSeconds < 0)
        {
            timetext = Translation.OverTimeBroadcast;

            foreach (Player player in Player.GetPlayers().Where(r => r.Role is RoleTypeId.ClassD))
            {
                if (player.IsWithoutItems)
                {
                    player.Kill(Translation.Died);
                }
            }
        }
        // A second life for dead players
        else if (Config.SecondLifeInSeconds == EventTime.TotalSeconds)
        {
            foreach (Player runner in Player.GetPlayers().Where(r => r.Role is RoleTypeId.Spectator))
            {
                Extensions.SetRole(runner, RoleTypeId.ClassD, RoleSpawnFlags.None);
                runner.Position = runnerSpawns.RandomItem().transform.position;
                runner.ReceiveHint(Translation.SecondLifeHint, 5);
            }
        }
        
        string text = Translation.CycleBroadcast;
        text = text.Replace("{name}", Name);
        text = text.Replace("{runnerCount}",$"{Player.GetPlayers().Count(r => r.Role is RoleTypeId.ClassD)}");
        text = text.Replace("{deathCount}", $"{Player.GetPlayers().Count(r => r.Role is RoleTypeId.Scientist)}");
        text = text.Replace("{time}", timetext);

        Extensions.Broadcast(text, 1);
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        
        if (Player.GetPlayers().Count(r => r.Role is RoleTypeId.ClassD) == 0)
        {
            text = Translation.DeathWinBroadcast.Replace("{name}", Name);
        }
        else
        {
            text = Translation.RunnerWinBroadcast.Replace("{name}", Name);
        }

        Extensions.Broadcast(text, 10);
    }
}