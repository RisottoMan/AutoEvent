using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Interfaces;
using MapGeneration.Distributors;
using AutoEvent.Games.Football;
using Exiled.API.Features;

namespace AutoEvent.Games.Jail;
public class Plugin : Event<Config, Translation>, IEventMap
{
    public override string Name { get; set; } = "Simon's Prison";
    public override string Description { get; set; } = "Jail mode from CS 1.6, in which you need to hold events [VERY HARD]";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "jail";
    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "Jail", 
        Position = new Vector3(50, 40, 66),
        IsStatic = false
    };
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
    public override EventFlags EventHandlerSettings { get; set; } = EventFlags.IgnoreDroppingItem;
    protected override float FrameDelayInSeconds { get; set; } = 0.5f;
    private EventHandler _eventHandler;
    internal GameObject Button { get; private set; }
    internal GameObject PrisonerDoors { get; private set; }
    internal Dictionary<Player, int> Deaths { get; set; } 
    internal List<GameObject> SpawnPoints { get; set; }
    private List<GameObject> _doors;
    private GameObject _ball;

    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Shooting += _eventHandler.OnShooting;
        Exiled.Events.Handlers.Player.Dying += _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.InteractingLocker += _eventHandler.OnInteractingLocker;
    }

    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Shooting -= _eventHandler.OnShooting;
        Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.InteractingLocker -= _eventHandler.OnInteractingLocker;
        
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        Deaths = new Dictionary<Player, int>();
        SpawnPoints = new List<GameObject>();
        _doors = new List<GameObject>();

        foreach (var obj in MapInfo.Map.AttachedBlocks)
        {
            switch (obj.name)
            {
                case string str when str.Contains("Spawnpoint"): SpawnPoints.Add(obj); break;
                case "Button": Button = obj; break;
                case "Ball":
                {
                    _ball = obj;
                    _ball.AddComponent<BallComponent>();
                    break;
                }
                case "Door":
                {
                    obj.AddComponent<DoorComponent>();
                    _doors.Add(obj);
                    break;
                }
                case "PrisonerDoors":
                {
                    PrisonerDoors = obj;
                    PrisonerDoors.AddComponent<JailerComponent>();
                    break;
                }
            }
        }

        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.PrisonerLoadouts);
            player.Position = SpawnPoints.Where(r => r.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }

        foreach (Player ply in Config.JailorRoleCount.GetPlayers(true))
        {
            ply.GiveLoadout(Config.JailorLoadouts, LoadoutFlags.IgnoreWeapons);
            ply.Position = SpawnPoints.Where(r => r.name == "SpawnpointMtf").ToList().RandomItem().transform.position;
        }
        
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 15; time > 0; time--)
        {
            foreach (Player player in Player.List)
            {
                player.ClearBroadcasts();
                if (player.HasLoadout(Config.JailorLoadouts))
                {
                    player.Broadcast(1, Translation.Start.Replace("{name}", Name).Replace("{time}", time.ToString("00")));
                }
                else
                {
                    player.Broadcast(1, Translation.StartPrisoners.Replace("{name}", Name).Replace("{time}", time.ToString("00")));
                }
            }
            
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override bool IsRoundDone()
    {
        return !(Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0 && Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0);
    }

    protected override void ProcessFrame()
    {
        string dClassCount = Player.List.Count(r => r.Role == RoleTypeId.ClassD).ToString();
        string mtfCount = Player.List.Count(r => r.Role.Team == Team.FoundationForces).ToString();
        string time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";

        foreach (Player player in Player.List)
        {
            foreach (GameObject doorComponent in _doors)
            {
                if (Vector3.Distance(doorComponent.transform.position, player.Position) < 3)
                {
                    doorComponent.GetComponent<DoorComponent>().Open();
                }
            }
            
            if (Vector3.Distance(_ball.transform.position, player.Position) < 2)
            {
                _ball.gameObject.TryGetComponent(out Rigidbody rig);
                rig.AddForce(player.GameObject.transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
            }

            player.ClearBroadcasts();
            player.Broadcast(1, Translation.Cycle.
                Replace("{name}", Name).
                Replace("{dclasscount}", dClassCount).
                Replace("{mtfcount}", mtfCount).Replace("{time}", time));
        }
    }

    protected override void OnFinished()
    {
        if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
        {
            Extensions.Broadcast(Translation.PrisonersWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
        }

        if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0)
        {
            Extensions.Broadcast(Translation.JailersWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
        }   
    }
}