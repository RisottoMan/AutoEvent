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
        public string Name => "Тюрьма Саймона";
        public string Description => "Режим Jail, в котором нужно проводить мероприятия.";
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
            // Запуск музыки
            //Extensions.PlayAudio("Jail.ogg", 15, false, "Инструкция");
            // включить огонь по своим
            Server.FriendlyFire = true;
            // Создание кнопки
            Button = new GameObject("button");
            Button.transform.position = GameMap.Position + new Vector3(21.88927f, -6.554526f, -2.148565f);
            // Запуск ивента
            OnEventStarted();
        }
        public void OnEventStarted()
        {
            for (int i = 0; i <= Player.List.Count() / 10; i++)
            {
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
                if (player.Role.Team != Team.FoundationForces)
                {
                    player.Role.Set(RoleTypeId.ClassD);
                    player.Position = GameMap.Position + JailRandom.GetRandomPosition();
                }
            }
            Timing.RunCoroutine(OnEventRunning(), "jail_run");
        }
        public IEnumerator<float> OnEventRunning()
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

                Extensions.Broadcast($"<size=20><color=red>Тюрьма Саймона</color>\n" +
                    $"<color=yellow>Зеки: {Player.List.Count(r => r.Role == RoleTypeId.ClassD)}</color> || " +
                    $"<color=cyan>Охраники: {Player.List.Count(r => r.Role.Team == Team.FoundationForces)}</color>\n" +
                    $"<color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></size>", 1);

                yield return Timing.WaitForSeconds(0.5f);
                EventTime += TimeSpan.FromSeconds(0.5f);
            }
            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast($"<color=red><b><i>Победа Заключенных</i></b></color>\n" +
                    $"<color=red>{EventTime.Minutes}:{EventTime.Seconds}</color>", 10);
            }
            if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast($"<color=blue><b><i>Победа Охранников</i></b></color>\n" +
                    $"<color=red>{EventTime.Minutes}:{EventTime.Seconds}</color>", 10);
            }
            OnStop();
            yield break;
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
