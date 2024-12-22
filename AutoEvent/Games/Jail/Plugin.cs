using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Interfaces;
using MapGeneration.Distributors;
using Event = AutoEvent.Interfaces.Event;
using AutoEvent.Games.Football;
using Exiled.API.Features;

namespace AutoEvent.Games.Jail;
public class Plugin : Event, IEventMap
{
    public override string Name { get; set; } = "Simon's Prison";
    public override string Description { get; set; } = "Jail mode from CS 1.6, in which you need to hold events [VERY HARD]";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "jail";
    [EventConfig]
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "Jail", 
        Position = new Vector3(90f, 1030f, -43.5f),
        IsStatic = false
    };
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
    public override EventFlags EventHandlerSettings { get; set; } = EventFlags.IgnoreDroppingItem;
    protected override float FrameDelayInSeconds { get; set; } = 0.5f;
    protected override float PostRoundDelay { get; set; } = 10f;
    private EventHandler _eventHandler { get; set; }
    internal GameObject Button { get; private set; }
    internal GameObject PrisonerDoors { get; private set; }
    internal Locker WeaponLocker { get; private set; }
    internal Locker Medical { get; private set; }
    internal Locker Adrenaline { get; private set; }
    internal Dictionary<Player, int> Deaths { get; set; } 
    internal JailLockdownSystem JailLockdownSystem { get; set; }
    private List<GameObject> _doors;
    private GameObject _ball;

    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        JailLockdownSystem = new JailLockdownSystem(this);
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
        JailLockdownSystem = null;
    }

    protected override void OnStart()
    {
        Deaths = new Dictionary<Player, int>();
        _doors = new List<GameObject>();

        foreach (var obj in MapInfo.Map.AttachedBlocks)
        {
            try
            {
                switch (obj.name)
                {
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
                    case "RifleRack":
                    {
                        var locker = obj.GetComponent<Locker>();
                        if (locker is not null)
                        {
                            WeaponLocker = locker;
                        }

                        break;
                    }
                    case "CabinetMedkit":
                    {
                        Locker medical = obj.GetComponent<Locker>();
                        if (medical is not null)
                        {
                            Medical = medical;
                        }
                        
                        break;
                    }
                    case "CabinetAdrenaline":
                    {
                        Locker adrenaline = obj.GetComponent<Locker>();
                        if (adrenaline is not null)
                        {
                            Adrenaline = adrenaline;
                        }
                        
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"An error has occured at JailPlugin.OnStart()", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
            }

        }

        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.PrisonerLoadouts);
            try
            {
                player.Position = JailRandom.GetRandomPosition(MapInfo.Map, false);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"An error has occured while trying to get a random spawnpoint.",
                    LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}");
            }
        }

        foreach (Player ply in Config.JailorRoleCount.GetPlayers(true))
        {
            ply.GiveLoadout(Config.JailorLoadouts, LoadoutFlags.IgnoreWeapons);
            try
            {
                ply.Position = JailRandom.GetRandomPosition(MapInfo.Map, true);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"An error has occured while trying to get a random spawnpoint.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}");
            }
        }
        
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        List<Player> jailors = new List<Player>();
        foreach (Player ply in Player.List)
        {
            if (ply.HasLoadout(Config.JailorLoadouts))
                jailors.Add(ply);
        }
        for (int time = 15; time > 0; time--)
        {
            Extensions.Broadcast(Translation.StartPrisoners.Replace("{name}", Name).Replace("{time}", time.ToString("00")), 1);
            foreach (Player ply in jailors)
            {
                if (ply is null)
                {
                    jailors.Remove(ply);
                    continue;
                }
                ply.ClearBroadcasts();
                ply.Broadcast(1, Translation.Start.Replace("{name}", Name).Replace("{time}", time.ToString("00")));
            }
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override bool IsRoundDone()
    {
        // At least one NTF is alive &&
        // At least one Class D is alive
        return !(Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0 &&
               Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0);
    }

    protected override void ProcessFrame()
    {
        string dClassCount = Player.List.Count(r => r.Role == RoleTypeId.ClassD).ToString();
        string mtfCount = Player.List.Count(r => r.Role.Team == Team.FoundationForces).ToString();
        string time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";

        foreach (Player player in Player.List)
        {
            if (Vector3.Distance(_ball.transform.position, player.Position) < 2)
            {
                _ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                rig.AddForce(player.GameObject.transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
            }

            player.ClearBroadcasts();
            player.Broadcast(1, Translation.Cycle.
                Replace("{name}", Name).
                Replace("{dclasscount}", dClassCount).
                Replace("{mtfcount}", mtfCount).Replace("{time}", time));
        }
        
        foreach (var doorComponent in _doors)
        {
            var doorTransform = doorComponent.transform;

            bool lockdownActive = JailLockdownSystem.LockDownActive;
            foreach (Player player in Player.List)
            {
                /*
                if (Config.LockdownSettings.LockdownLocksGatesAsWell && lockdownActive && !player.HasKeycardLevel(KeycardPermissions.Checkpoints | KeycardPermissions.BypassMode))
                {
                    continue;
                }
                */
                if (Vector3.Distance(doorTransform.position, player.Position) < 3)
                {
                    doorComponent.GetComponent<DoorComponent>().Open();
                }
            }
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

public enum BypassLevel
{
    None = 1,
    Guard = 1,
    ContainmentEngineer = 2,
    O5 = 2,
    BypassMode = 5
}
