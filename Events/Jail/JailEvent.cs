using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AutoEvent.Events
{
    internal class JailEvent : IEvent
    {
        public string Name => AutoEvent.Singleton.Translation.JailName;
        public string Description => AutoEvent.Singleton.Translation.JailDescription;
        public string Color => "FFFF00";
        public string CommandName => "jail";
        public static SchematicObject GameMap { get; set; }
        public static GameObject Button { get; set; }
        public static Dictionary<GameObject, float> JailerDoorsTime { get; set; } = new Dictionary<GameObject, float>();
        public static TimeSpan EventTime { get; set; }
        public static bool isDoorsOpen = false;

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Shooting += JailHandler.OnShootEvent;
            Exiled.Events.Handlers.Player.InteractingLocker += JailHandler.OnInteractLocker;
            Exiled.Events.Handlers.Server.RespawningTeam += JailHandler.OnTeamRespawn;
            OnWaitingEvent();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Shooting -= JailHandler.OnShootEvent;
            Exiled.Events.Handlers.Player.InteractingLocker -= JailHandler.OnInteractLocker;
            Exiled.Events.Handlers.Server.RespawningTeam -= JailHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }
        public void OnWaitingEvent()
        {
            GameMap = Extensions.LoadMap("Jail", new Vector3(115.5f, 1030f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            // Setup instruction
            //Extensions.PlayAudio("Jail.ogg", 15, false, "Instruction");
            Server.FriendlyFire = true;
            // The button for the shot
            Button = new GameObject("button");
            Button.transform.position = GameMap.Position + new Vector3(21.88927f, -6.554526f, -2.148565f);

            OnEventStarted();
        }
        public void OnEventStarted()
        {
            for (int i = 0; i <= Player.List.Count() / 10; i++)
            {
                // Random player. You have to become one yourself through the admin panel.
                var jailer = Player.List.ToList().RandomItem();
                jailer.Role.Set(RoleTypeId.NtfCaptain);
                jailer.Position = GameMap.Position + new Vector3(13.506f, -10f, -13.192f);
                jailer.ResetInventory(new List<ItemType>
                    {
                        ItemType.GunE11SR,
                        ItemType.GunCOM18
                    });
            }
            foreach (Player player in Player.List)
            {
                if (player.Role != RoleTypeId.NtfCaptain)
                {
                    player.Role.Set(RoleTypeId.ClassD);
                    player.Position = GameMap.Position + JailRandom.GetRandomPosition();
                }
            }
            Timing.RunCoroutine(OnEventRunning(), "jail_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            EventTime = new TimeSpan(0, 0, 0);
            // Countdown before the start of the game
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(trans.JailBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }
            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0 && Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0)
            {
                PhysicDoors();

                string dClassCount = Player.List.Count(r => r.Role == RoleTypeId.ClassD).ToString();
                string mtfCount = Player.List.Count(r => r.Role.Team == Team.FoundationForces).ToString();
                string time = $"{ EventTime.Minutes }:{ EventTime.Seconds }";
                Extensions.Broadcast(trans.JailCycle.Replace("{name}", Name).Replace("{dclasscount}", dClassCount).Replace("{mtfcount}", mtfCount).Replace("{time}", time), 1);
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
        public void PhysicDoors()
        {
            // Physics of door control.
            foreach (var door in Object.FindObjectsOfType<PrimitiveObject>())
            {
                if (door.name == "Door")
                {
                    if (JailerDoorsTime.ContainsKey(door.gameObject))
                    {
                        if (JailerDoorsTime[door.gameObject] <= 0)
                        {
                            door.Position += new Vector3(0f, 4f, 0f);
                            JailerDoorsTime.Remove(door.gameObject);
                        }
                        else JailerDoorsTime[door.gameObject] -= 0.5f;
                    }

                    foreach (Player player in Player.List)
                    {
                        if (Vector3.Distance(door.transform.position, player.Position) < 3)
                        {
                            door.Position += new Vector3(0f, -4f, 0f);

                            if (!JailerDoorsTime.ContainsKey(door.gameObject))
                            {
                                JailerDoorsTime.Add(door.gameObject, 2f);
                            }
                        }
                    }
                }
            }
        }
        public void EventEnd()
        {
            isDoorsOpen = false;
            Server.FriendlyFire = false;

            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            JailerDoorsTime.Clear();
            GameObject.Destroy(Button);
            Extensions.StopAudio();
        }
    }
}
