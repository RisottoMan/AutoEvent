using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Deathrun.Features;
using AutoEvent.Interfaces;
using CommandSystem;
using PlayerRoles;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Deathrun;
public class Plugin : Event, IEventMap, IInternalEvent, IHiddenCommand
{
    public override string Name { get; set; } = "Death Run";
    public override string Description { get; set; } = "Go to the end, avoiding death-activated trap along the way";
    public override string Author { get; set; } = "RisottoMan/code & karorogunso/map";
    public override string CommandName { get; set; } = "deathrun";
    public override Version Version { get; set; } = new Version(1, 0, 0);
    [EventConfig]
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public MapInfo MapInfo { get; set; } = new MapInfo()
    { 
        MapName = "Deathrun", 
        Position = new Vector3(15f, 1030f, -43.68f)
    };
    private EventHandler _eventHandler { get; set; }
    private List<GameObject> _interactableObjects { get; set; }
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        EventManager.RegisterEvents(_eventHandler);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
        Players.PickUpItem += _eventHandler.OnPickUpItem;
    }

    protected override void UnregisterEvents()
    {
        EventManager.UnregisterEvents(_eventHandler);
        Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
        Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
        Players.DropItem -= _eventHandler.OnDropItem;
        Players.DropAmmo -= _eventHandler.OnDropAmmo;
        Players.PickUpItem -= _eventHandler.OnPickUpItem;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        // Get map objects for interactions
        List<GameObject> runnerSpawns = new();
        List<GameObject> deathSpawns = new();
        _interactableObjects = new();
        foreach (var block in MapInfo.Map.AttachedBlocks)
        {
            switch (block.name)
            {
                case "Spawnpoint-Runner": runnerSpawns.Add(block); break;
                case "Spawnpoint-Death": deathSpawns.Add(block); break;
                case "InteractableObject": _interactableObjects.Add(block); break;
                case "KillZone": block.AddComponent<DeathComponent>(); break;
                case "FinishZone": block.AddComponent<FinishComponent>(); break;
            }
        }
        
        // Making a random death-guy and teleport to spawnpoint
        for (int i = 0; Player.GetPlayers().Count() / 20 >= i; i++)
        {
            Player death = Player.GetPlayers().Where(r => r.Role != RoleTypeId.Scientist).ToList().RandomItem();
            Extensions.SetRole(death, RoleTypeId.Scientist, RoleSpawnFlags.None);
            death.Position = deathSpawns.RandomItem().transform.position;
        }
        
        // Teleport runners to spawnpoint
        foreach (Player runner in Player.GetPlayers().Where(r => r.Role != RoleTypeId.Scientist))
        {
            Extensions.SetRole(runner, RoleTypeId.ClassD, RoleSpawnFlags.None);
            runner.Position = runnerSpawns.RandomItem().transform.position;
        }
    }

    // Counting down the time to the start of the game
    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (float time = 1; time > 0; time--) // 10
        {
            Extensions.Broadcast($"{Translation.Name}" +
                                 $"Осторожно, снаружи смертельные ловушки\n" +
                                 $"Приготовьтесь бежать бегуны через {time} секунд", 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }
    
    // Destroy the wall so that players can start passing the map
    protected override void CountdownFinished()
    {
        GameObject.Destroy(MapInfo.Map.AttachedBlocks.First(r => r.name == "Wall"));
    }

    protected override bool IsRoundDone()
    {
        return false;
        return !(Player.GetPlayers().Count(r => r.Role is RoleTypeId.Scientist) > 0 &&
                 Player.GetPlayers().Count(r => r.Role is RoleTypeId.ClassD) > 0 &&
                 EventTime.TotalMinutes < 5); // ???
    }

    protected override void ProcessFrame()
    {
        string text = "{name}\n<color=yellow>{runnerCount}</color> | <color=red>{deathCount}</color>\nTime Left: {time}";
        TimeSpan timeleft = new TimeSpan(); //
        
        text = text.Replace("{name}", Name);
        text = text.Replace("{runnerCount}", Player.GetPlayers().Count(r => r.IsHuman).ToString());
        text = text.Replace("{deathCount}", Player.GetPlayers().Count(r => r.IsHuman).ToString());
        text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

        foreach (var player in Player.GetPlayers())
        {
            player.ClearBroadcasts();
            player.SendBroadcast(text, 1);
        }
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        int count = Player.GetPlayers().Count(r => r.Role is RoleTypeId.Scientist or RoleTypeId.ClassD);
        
        if (count == 0)
        {
            text = "<color=red>Death win</color>";
        }
        else if (count == 1)
        {
            Player player = Player.GetPlayers().First(r => r.Role is RoleTypeId.Scientist or RoleTypeId.ClassD);
            text = $"<color=yellow>Runner {player.Nickname} win</color>";
        }
        else
        {
            text = "<color=yellow>Runners win</color>";
        }

        Extensions.Broadcast(text, 10);
    }
}