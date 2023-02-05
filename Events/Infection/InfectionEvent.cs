using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events
{
    internal class InfectionEvent : IEvent
    {
        public string Name => "Заражение Зомби";
        public string Description => "Зомби режим, целью которого заразить всех игроков.";
        public string Color => "FF4242";
        public string CommandName => "zombie";
        public static SchematicObject GameMap { get; set; }
        public static TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += InfectionHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died += InfectionHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting += InfectionHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam += InfectionHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= InfectionHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died -= InfectionHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting -= InfectionHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam -= InfectionHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            // Обнуление Таймера
            EventTime = new TimeSpan(0, 0, 0);
            // Создание карты
            GameMap = Extensions.LoadMap("Zombie", new Vector3(115.5f, 1030f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            // Запуск музыки
            Extensions.PlayAudio("Zombie.ogg", 15, true, "Заражение");
            // Телепорт игроков
            foreach(Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                player.Position = GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
                player.ClearInventory();
            }
            Timing.RunCoroutine(TimingBeginEvent($"Заражение", 15), "zombie_time");
        }
        // Отсчет до начала ивента
        public IEnumerator<float> TimingBeginEvent(string eventName, float time)
        {
            for (float _time = time; _time > 0; _time--)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(1, $"<color=#D71868><b><i>{eventName}</i></b></color>\n<color=#ABF000>До начала ивента осталось <color=red>{_time}</color> секунд.</color>");
                yield return Timing.WaitForSeconds(1f);
            }
            SpawnZombie();
            yield break;
        }
        // Спавн зомби
        public void SpawnZombie()
        {
            Player.List.ToList().RandomItem().Role.Set(RoleTypeId.Scp0492);
            Timing.RunCoroutine(OnEventRunning(), "infect_run");
        }
        // Ивент начался - отсчет времени и колво людей
        public IEnumerator<float> OnEventRunning()
        {
            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 1)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(1, $"<color=#D71868><b><i>Заражение</i></b></color>\n" +
                    $"<color=yellow>Осталось людей: <color=green>{Player.List.Count(r => r.Role == RoleTypeId.ClassD)}</color></color>\n" +
                    $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>");

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
            Timing.RunCoroutine(DopTime(), "EventBeginning");
            yield break;
        }
        // Если останется один человек, то обратный отсчет
        public IEnumerator<float> DopTime()
        {
            for (int doptime = 30; doptime > 0; doptime--)
            {
                if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0) break;

                Map.ClearBroadcasts();
                Map.Broadcast(1, $"Дополнительное время: {doptime}\n" +
                $"<color=yellow>Остался <b><i>Последний</i></b> человек!</color>\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>");

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
            if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(10, $"<color=red>Зомби Победили!</color>\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>");
            }
            else
            {
                Map.ClearBroadcasts();
                Map.Broadcast(10, $"<color=yellow><color=#D71868><b><i>Люди</i></b></color> Победили!</color>\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>");
            }
            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
