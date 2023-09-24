using AutoEvent.API.Schematic.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using MapGeneration.Distributors;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Jail
{
    public class Plugin : Event, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.JailTranslate.JailName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.JailTranslate.JailDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "jail";
        [EventConfig]
        public JailConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Jail", Position = new Vector3(90f, 1030f, -43.5f), };
        protected override float FrameDelayInSeconds { get; set; } = 0.5f;
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private JailTranslate Translation { get; set; }
        internal GameObject Button { get; private set; }
        internal GameObject PrisonerDoors { get; private set; }
        internal Locker WeaponLocker { get; private set; }
        internal Locker Medical { get; private set; }
        internal Locker Adrenaline { get; private set; }
        internal Dictionary<Player, int> Deaths { get; set; } 
        internal float LockDownCooldown { get; set; }
        internal float LockDownRemainingDuration { get; set; }
        internal bool LockDownActive => !PrisonerDoors.GetComponent<JailerComponent>().IsOpen;

        private List<GameObject> _doors;
        private GameObject _ball;
        private float _remainingAutoReleaseTime;
        

        protected override void RegisterEvents()
        {
            Translation = new JailTranslate();
            EventHandler = new EventHandler(this);
            EventManager.RegisterEvents(EventHandler);
            Players.PlayerDying += EventHandler.OnPlayerDying;
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Players.LockerInteract += EventHandler.OnLockerInteract;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Players.LockerInteract -= EventHandler.OnLockerInteract;
            Players.PlayerDying -= EventHandler.OnPlayerDying;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            Deaths = new Dictionary<Player, int>();
            _remainingAutoReleaseTime = Config.AutoReleaseDelayInSeconds;
            Server.FriendlyFire = true;

            _doors = new List<GameObject>();

            foreach (var obj in MapInfo.Map.AttachedBlocks)
            {
                try
                {

                    switch (obj.name)
                    {
                        case "Button":
                        {
                            Button = obj;
                        }
                            break;
                        case "Ball":
                        {
                            _ball = obj;
                            _ball.AddComponent<BallComponent>();
                        }
                            break;
                        case "Door":
                        {
                            obj.AddComponent<DoorComponent>();
                            _doors.Add(obj);
                        }
                            break;
                        case "PrisonerDoors":
                        {
                            PrisonerDoors = obj;
                            PrisonerDoors.AddComponent<JailerComponent>();
                        }
                            break;
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
                        }
                            break;
                        case "CabinetAdrenaline":
                        {
                            Locker adrenaline = obj.GetComponent<Locker>();
                            if (adrenaline is not null)
                            {
                                Adrenaline = adrenaline;
                            }
                        }
                            break;
                    }
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"An error has occured at JailPlugin.OnStart()", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                }

            }

            // todo Need add check permission

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PrisonerLoadouts);
                player.Position = JailRandom.GetRandomPosition(MapInfo.Map, false);
            }

            foreach (Player ply in Config.JailorRoleCount.GetPlayers())
            {
                ply.GiveLoadout(Config.JailorLoadouts, LoadoutFlags.IgnoreWeapons );
                ply.Position = JailRandom.GetRandomPosition(MapInfo.Map, true);
            }
            
        }

        
        internal bool ToggleLockdown(bool autoRelease = false, BypassLevel bypassLevel = BypassLevel.None)
        {
            if (autoRelease)
            {
                DebugLogger.LogDebug("Auto Releasing Lockdown.");
                _remainingAutoReleaseTime = -1;
                if(LockDownActive)
                    PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
                return true;
            }
            
            // If a lockdown is triggered, release it and add a cooldown.
            if (LockDownActive)
            {
                DebugLogger.LogDebug("Releasing Lockdown.");
                ToggleLockdown();
                LockDownCooldown = Config.LockdownCooldownDurationInSeconds;
                LockDownRemainingDuration = 0;
                return true;
            }
            if (bypassLevel == BypassLevel.BypassMode)
            {
                DebugLogger.LogDebug("Bypass Mode - Skipping Cooldown. Duration will be maximum.");
                ToggleLockdown();
                LockDownRemainingDuration = float.MaxValue;
                return true;
            }
            // If cooldown is not done, wait.
            if (!LockDownActive && LockDownCooldown > 0)
            {
                DebugLogger.LogDebug("Cooldown is not finished. Cannot trigger lockdown.");
                return false;
            }

            // Allow Locking Down Doors
            if (!LockDownActive && LockDownCooldown <= 0)
            {
                DebugLogger.LogDebug("Cooldown is finished, Triggering lockdown.");
                ToggleLockdown();
                LockDownCooldown = 0;
                LockDownRemainingDuration = Config.LockdownDurationInSeconds * (int) bypassLevel;
                return true;
            }

            DebugLogger.LogDebug("Error lockdown trigger somehow failed.");
            return false;
        }
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.JailBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
                if(_remainingAutoReleaseTime > 0)
                    _remainingAutoReleaseTime -= 1;
                if (_remainingAutoReleaseTime == 0)
                {
                }
            }
        }

        protected override bool IsRoundDone()
        {
            // At least one NTF is alive &&
            // At least one Class D is alive
            return !(Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0 &&
                   Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > 0);
        }

        protected override void ProcessFrame()
        {
            if (_remainingAutoReleaseTime > 0)
            {
                _remainingAutoReleaseTime -= this.FrameDelayInSeconds;
            }

            if (_remainingAutoReleaseTime == 0)
            {
                ToggleLockdown(true);
            }

            if (LockDownCooldown > 0)
            {
                LockDownCooldown -= this.FrameDelayInSeconds;
            }
            
            if (LockDownRemainingDuration > 0)
            {
                LockDownRemainingDuration -= this.FrameDelayInSeconds;
            }

            if (LockDownRemainingDuration == 0)
            {
                ToggleLockdown();
            }
            
            string dClassCount = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD).ToString();
            string mtfCount = Player.GetPlayers().Count(r => r.Team == Team.FoundationForces).ToString();
            string time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";

            foreach (Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(_ball.transform.position, player.Position) < 2)
                {
                    _ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                    rig.AddForce(player.GameObject.transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
                }

                player.ClearBroadcasts();
                player.SendBroadcast(
                    Translation.JailCycle.Replace("{name}", Name).Replace("{dclasscount}", dClassCount)
                        .Replace("{mtfcount}", mtfCount).Replace("{time}", time), 1);
            }

            foreach (var doorComponent in _doors)
            {
                var doorTransform = doorComponent.transform;

                foreach (Player player in Player.GetPlayers())
                {
                    if (Vector3.Distance(doorTransform.position, player.Position) < 3)
                    {
                        doorComponent.GetComponent<DoorComponent>().Open();
                    }
                }
            }
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.JailPrisonersWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(Translation.JailJailersWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }   
        }

        protected override void OnCleanup()
        {
            Server.FriendlyFire = AutoEvent.IsFriendlyFireEnabledByDefault;
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
