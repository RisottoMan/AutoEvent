using AdminToys;
using AutoEvent.Interfaces;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Enums;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AutoEvent.Events
{
    internal class JailEvent : IEvent
    {
        public string Name => "Тюрьма Саймона";
        public string Description => "Режим Jail, в котором нужно проводить мероприятия.";
        public string Color => "FFFF00";
        public string CommandName => "jail";
        public static SchematicObject GameMap { get; set; }
        public static GameObject Button { get; set; }
        public static Dictionary<GameObject, float> JailerDoorsTime { get; set; } = new Dictionary<GameObject, float>();
        public static TimeSpan EventTime { get; set; }
        public static bool isDoorsOpen = false;
        public int Votes { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShootEvent;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInteractLocker;
            Exiled.Events.Handlers.Server.RespawningTeam += OnTeamRespawn;
            // Подготовка к ивенту
            OnWaitingEvent();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShootEvent;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractLocker;
            Exiled.Events.Handlers.Server.RespawningTeam -= OnTeamRespawn;

            Timing.CallDelayed(10f, () => EventEnd());
            //Plugin.ActiveEvent = null;
        }
        public void OnWaitingEvent()
        {
            // Создание карты
            GameMap = API.API.LoadMap("Jail", new Vector3(115.5f, 1030f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            // Запуск музыки
            //API.API.PlayAudio("Jail.ogg", 15, false, "Инструкция");
            // включить огонь по своим
            Server.FriendlyFire = true;
            // Создание кнопки
            Button = new GameObject("button");
            Button.transform.position = GameMap.gameObject.transform.position + new Vector3(21.88927f, -6.554526f, -2.148565f);
            // Запуск ивента
            OnEventStarted();
        }
        public void OnEventStarted()
        {
            for (int i = 0; i <= Player.List.Count() / 10; i++)
            {
                var jailer = Player.List.ToList().RandomItem();
                jailer.Role.Set(RoleTypeId.NtfCaptain, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                jailer.Position = GameMap.transform.position + new Vector3(13.506f, -10.9f, -13.192f);
                jailer.ResetInventory(new List<ItemType>
                    {
                        ItemType.GunE11SR,
                        ItemType.GunCOM18
                    });
            }
            foreach (Player player in Player.List)
            {
                if (player.Role.Team != Team.FoundationForces)
                {
                    player.Role.Set(RoleTypeId.ClassD, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = GameMap.gameObject.transform.position + RandomPosition();
                }
            }
            Timing.RunCoroutine(Cycle(), "jail_time");
        }
        public IEnumerator<float> Cycle()
        {
            // Обнуление таймера
            EventTime = new TimeSpan(0, 0, 0);
            // Отсчет обратного времени
            for (int time = 15; time > 0; time--)
            {
                Player.List.ToList().ForEach(player =>
                {
                    player.ClearBroadcasts();
                    player.Broadcast(new Exiled.API.Features.Broadcast($"<color=yellow>Ивент <color=red><b><i>Тюрьма Саймона</i></b></color>\n" +
                        $"<i>Открыть двери игрокам, стрельнув в кнопку</i>\n" +
                        $"До начала: <color=red>{time}</color> секунд</color>", 1));
                });
                yield return Timing.WaitForSeconds(1f);
            }
            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0 && Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0)
            {
                PhysicDoors();
                Player.List.ToList().ForEach(player =>
                {
                    player.ClearBroadcasts();
                    player.Broadcast(new Exiled.API.Features.Broadcast($"<size=20><color=red>Тюрьма Саймона</color>\n" +
                    $"<color=yellow>Зеки: {Player.List.Count(r => r.Role == RoleTypeId.ClassD)}</color> || " +
                    $"<color=cyan>Охраники: {Player.List.Count(r => r.Role.Team == Team.FoundationForces)}</color>\n" +
                    $"<color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></size>", 1));
                });
                yield return Timing.WaitForSeconds(0.5f);
                EventTime += TimeSpan.FromSeconds(0.5f);
            }
            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(new Exiled.API.Features.Broadcast($"<color=red><b><i>Победа Заключенных</i></b></color>\n" +
                    $"<color=red>{EventTime.Minutes}:{EventTime.Seconds}</color>", 10));
            }
            if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(new Exiled.API.Features.Broadcast($"<color=blue><b><i>Победа Охранников</i></b></color>\n" +
                    $"<color=red>{EventTime.Minutes}:{EventTime.Seconds}</color>", 10));
            }
            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            isDoorsOpen = false;
            Server.FriendlyFire = false;

            API.API.CleanUpAll();
            API.API.TeleportEnd();
            API.API.UnLoadMap(GameMap);
            JailerDoorsTime.Clear();
            GameObject.Destroy(Button);
            //API.API.StopAudio();
        }
        public Vector3 RandomPosition()
        {
            Vector3 position = new Vector3(0, 0, 0);
            switch (Random.Range(0, 15))
            {
                case 0: position = new Vector3(6.99f, -5.396f, 17.18f); break;
                case 1: position = new Vector3(14.36f, -5.396f, 17.18f); break;
                case 2: position = new Vector3(21.49f, -5.396f, 17.18f); break;
                case 3: position = new Vector3(28.82f, -5.396f, 17.18f); break;
                case 4: position = new Vector3(36.47f, -5.396f, 17.18f); break;
                case 5: position = new Vector3(6.99f, -8.686f, 17.18f); break;
                case 6: position = new Vector3(14.36f, -8.686f, 17.18f); break;
                case 7: position = new Vector3(21.49f, -8.686f, 17.18f); break;
                case 8: position = new Vector3(28.82f, -8.686f, 17.18f); break;
                case 9: position = new Vector3(36.47f, -8.686f, 17.18f); break;
                case 10: position = new Vector3(6.99f, -12, 17.18f); break;
                case 11: position = new Vector3(14.36f, -12, 17.18f); break;
                case 12: position = new Vector3(21.49f, -12, 17.18f); break;
                case 13: position = new Vector3(28.82f, -12, 17.18f); break;
                case 14: position = new Vector3(36.47f, -12, 17.18f); break;
            }
            return position;
        }
        public void PhysicDoors()
        {
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
        // Ивенты
        public void OnInteractLocker(InteractingLockerEventArgs ev)
        {
            ev.IsAllowed = false;
            if (ev.Locker.StructureType == MapGeneration.Distributors.StructureType.LargeGunLocker)
            {
                ev.Player.ResetInventory(new List<ItemType>
                {
                    ItemType.GunE11SR,
                    ItemType.GunCOM18
                });
            }
            if (ev.Locker.StructureType == MapGeneration.Distributors.StructureType.SmallWallCabinet)
            {
                if (Vector3.Distance(ev.Player.Position, GameMap.gameObject.transform.position + new Vector3(17.855f, -12.43052f, -23.632f)) < 2)
                {
                    ev.Player.AddAhp(100, 100, 0);
                }
                else
                {
                    ev.Player.Health = ev.Player.MaxHealth;
                }
            }
        }
        public void OnShootEvent(ShootingEventArgs ev)
        {
            RaycastHit raycastHit;
            if (!Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.transform.position, ev.Player.ReferenceHub.PlayerCameraReference.transform.forward, out raycastHit, 100f))
            {
                return;
            }
            if (Vector3.Distance(raycastHit.transform.gameObject.transform.position, Button.gameObject.transform.position) < 3)
            {
                if (isDoorsOpen)
                {
                    foreach (var obj in Object.FindObjectsOfType<PrimitiveObject>())
                    {
                        if (obj.name == "PrisonerDoor") obj.Position += new Vector3(2.2f, 0, 0);
                    }
                    isDoorsOpen = false;
                }
                else
                {
                    foreach (var obj in Object.FindObjectsOfType<PrimitiveObject>())
                    {
                        if (obj.name == "PrisonerDoor") obj.Position += new Vector3(-2.2f, 0, 0);
                    }
                    isDoorsOpen = true;
                }
            }
        }
        public static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
