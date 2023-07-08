using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Events.Jail
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.JailName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.JailDescription;
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "jail";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }
        public GameObject Button { get; set; }
        public List<GameObject> Doors { get; set; }
        public List<GameObject> JailerDoors { get; set; }
        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.Shooting += _eventHandler.OnShootEvent;
            Exiled.Events.Handlers.Player.InteractingLocker += _eventHandler.OnInteractLocker;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            OnWaitingEvent();
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Shooting -= _eventHandler.OnShootEvent;
            Exiled.Events.Handlers.Player.InteractingLocker -= _eventHandler.OnInteractLocker;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }
        public void OnWaitingEvent()
        {
            GameMap = Extensions.LoadMap("Jail", new Vector3(90f, 1030f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Server.FriendlyFire = true;

            Button = GameMap.AttachedBlocks.First(r => r.name == "Button");
            Doors = new List<GameObject>();
            JailerDoors = new List<GameObject>();

            foreach (var obj in GameMap.AttachedBlocks)
            {
                if (obj.name == "Door")
                {
                    obj.AddComponent<DoorComponent>();
                    Doors.Add(obj);
                }

                if (obj.name == "PrisonerDoor")
                {
                    obj.AddComponent<JailerComponent>();
                    JailerDoors.Add(obj);
                }
            }

            foreach(Player player in Player.List)
            {
                if (player.Sender.CheckPermission("ev.run"))
                {
                    player.Role.Set(RoleTypeId.NtfCaptain, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = JailRandom.GetRandomPosition(GameMap, true);
                    player.AddItem(new List<ItemType> { ItemType.GunE11SR, ItemType.GunCOM18 });
                }
            }
            
            foreach (Player player in Player.List)
            {
                if (Player.List.Count(r => r.IsNTF) < 0)
                {
                    player.Role.Set(RoleTypeId.NtfCaptain, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = JailRandom.GetRandomPosition(GameMap, true);
                    player.AddItem(new List<ItemType> { ItemType.GunE11SR, ItemType.GunCOM18 });
                }
                else if (player.Role != RoleTypeId.NtfCaptain)
                {
                    player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = JailRandom.GetRandomPosition(GameMap, false);
                }
            }
            
            Timing.RunCoroutine(OnEventRunning(), "jail_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            EventTime = new TimeSpan(0, 0, 0);

            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(trans.JailBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0 && Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0)
            {
                string dClassCount = Player.List.Count(r => r.Role == RoleTypeId.ClassD).ToString();
                string mtfCount = Player.List.Count(r => r.Role.Team == Team.FoundationForces).ToString();
                string time = $"{EventTime.Minutes}:{EventTime.Seconds}";
                Extensions.Broadcast(trans.JailCycle.Replace("{name}", Name).Replace("{dclasscount}", dClassCount).Replace("{mtfcount}", mtfCount).Replace("{time}", time), 1);

                foreach (var doorComponent in Doors)
                {
                    var doorTransform = doorComponent.transform;

                    foreach (Player player in Player.List)
                    {
                        if (Vector3.Distance(doorTransform.position, player.Position) < 3)
                        {
                            doorComponent.GetComponent<DoorComponent>().Open();
                        }
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);
                EventTime += TimeSpan.FromSeconds(0.5f);
            }

            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(trans.JailPrisonersWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
            }

            if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(trans.JailJailersWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Server.FriendlyFire = false;
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
