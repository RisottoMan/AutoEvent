using AutoEvent.API.Schematic.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Games.Football.Features;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Jail
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.JailName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.JailDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Jail";
        public override string CommandName { get; set; } = "jail";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }
        public List<GameObject> Doors { get; set; }
        public GameObject Ball { get; set; }
        public GameObject Button { get; set; }
        public GameObject PrisonerDoors { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;

            OnWaitingEvent();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnWaitingEvent()
        {
            GameMap = Extensions.LoadMap(MapName, new Vector3(90f, 1030f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Server.FriendlyFire = true;

            Doors = new List<GameObject>();

            foreach (var obj in GameMap.AttachedBlocks)
            {
                switch(obj.name)
                {
                    case "Button": { Button = obj; } break;
                    case "Ball":
                        {
                            Ball = obj;
                            Ball.AddComponent<BallComponent>();
                        }
                        break;
                    case "Door":
                        {
                            obj.AddComponent<DoorComponent>();
                            Doors.Add(obj);
                        }
                        break;
                    case "PrisonerDoors":
                        {
                            PrisonerDoors = obj;
                            PrisonerDoors.AddComponent<JailerComponent>();
                        }
                        break;
                }
            }

            foreach(Player player in Player.GetPlayers())
            {
                /*
                if (player.Sender.CheckPermission("ev.run"))
                {
                    Extensions.SetRole(player, RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                    player.Position = JailRandom.GetRandomPosition(GameMap, true);
                    player.AddItem(new List<ItemType> { ItemType.GunE11SR, ItemType.GunCOM18 });
                }*/
            }

            foreach (Player player in Player.GetPlayers())
            {
                if (Player.GetPlayers().Count(r => r.IsNTF) < 0)
                {
                    Extensions.SetRole(player, RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                    player.Position = JailRandom.GetRandomPosition(GameMap, true);
                    player.AddItem(ItemType.GunE11SR);
                    player.AddItem(ItemType.GunCOM18);
                }
                else if (player.Role != RoleTypeId.NtfCaptain)
                {
                    Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                    player.Position = JailRandom.GetRandomPosition(GameMap, false);
                }
            }
            
            Timing.RunCoroutine(OnEventRunning(), "jail_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            EventTime = new TimeSpan(0, 0, 0);
            var translation = AutoEvent.Singleton.Translation;

            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(translation.JailBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0 && Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > 0)
            {
                string dClassCount = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD).ToString();
                string mtfCount = Player.GetPlayers().Count(r => r.Team == Team.FoundationForces).ToString();
                string time = $"{EventTime.Minutes}:{EventTime.Seconds}";

                foreach(Player player in Player.GetPlayers())
                {
                    if (Vector3.Distance(Ball.transform.position, player.Position) < 2)
                    {
                        Ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                        rig.AddForce(player.GameObject.transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
                    }

                    player.ClearBroadcasts();
                    player.SendBroadcast(translation.JailCycle.Replace("{name}", Name).
                        Replace("{dclasscount}", dClassCount).
                        Replace("{mtfcount}", mtfCount).
                        Replace("{time}", time), 1);
                }

                foreach (var doorComponent in Doors)
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

                yield return Timing.WaitForSeconds(0.5f);
                EventTime += TimeSpan.FromSeconds(0.5f);
            }

            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(translation.JailPrisonersWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
            }

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(translation.JailJailersWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
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
