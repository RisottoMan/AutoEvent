using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.HideAndSeek
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Догонялки [Testing]";
        public override string Description { get; set; } = "Надо догнать всех игроков на карте. [Testing]";
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "hide";
        public SchematicObject GameMap { get; set; }
        //public static Model Ledders { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died += _eventHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died -= _eventHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            
            // Обнуление Таймера
            EventTime = new TimeSpan(0, 0, 0);
            // Создание карты
            //GameMap = Extensions.LoadMap("HNS", new Vector3(115.5f, 1030f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            // Запуск музыки
            //Extensions.PlayAudio("Zombie.ogg", 15, true, "Заражение");
            // Телепорт игроков
            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                player.Position = GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
                player.ClearInventory();
            }
            Timing.RunCoroutine(TimingBeginEvent($"Догонялки", 30), "hns_run");
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
            SpawnCatcher();
            yield break;
        }
        // Спавн зомби
        public void SpawnCatcher()
        {
            var player = Player.List.ToList().RandomItem();
            player.Role.Set(RoleTypeId.NtfCaptain);
            player.ResetInventory(new List<ItemType> { ItemType.Jailbird });
            Timing.RunCoroutine(OnEventRunning(), "hns_new_run");
        }
        // Ивент начался - отсчет времени и колво людей
        public IEnumerator<float> OnEventRunning()
        {
            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 1)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(1, $"<color=#D71868><b><i>Догонялки</i></b></color>\n" +
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
                Map.Broadcast(10, $"<color=red>Догоняющие Победили!</color>\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>");
            }
            else
            {
                Map.ClearBroadcasts();
                Map.Broadcast(10, $"<color=yellow><color=#D71868><b><i>Д-Класс</i></b></color> Победил!</color>\n" +
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
