using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Survival
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Зомби Выживание [Testing]";
        public override string Description { get; set; } = "Выживание людей против зомби. [Alpha]";
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "survival";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            //_eventHandler = new EventHandler();
            /*
            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died += _eventHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            */
            OnEventStarted();
        }
        public override void OnStop()
        {
            /*
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died -= _eventHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            */
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            // Обнуление Таймера
            EventTime = new TimeSpan(0, 5, 0);
            // Создание карты
            GameMap = Extensions.LoadMap("Zm_Dust_World.json", new Vector3(115.5f, 1035f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            Extensions.PlayAudio("Survival.ogg", 5, false, "Выживание");
            // Телепорт игроков
            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.NtfSergeant, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.AssignInventory);
                //player.Position = GameMap.Position + RandomClass.GetRandomSpawn();
            }
            Timing.RunCoroutine(TimingBeginEvent($"Выживание", 15), "survival_run");
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

        public void SpawnZombie()
        {
            for(int i = 0; i <= Player.List.Count() / 10; i++)
            {
                Player.List.ToList().RandomItem().Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.Revived, RoleSpawnFlags.AssignInventory);
            }
            Timing.RunCoroutine(OnEventRunning(), "infect_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            while (Player.List.Count(r => r.Role == RoleTypeId.NtfSergeant) > 0 && Player.List.Count(r => r.Role == RoleTypeId.Scp0492) > 0 && EventTime.TotalSeconds > 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(1, $"<color=#D71868><b><i>Зомби Выживание</i></b></color>\n" +
                    $"<color=yellow>Осталось людей: <color=green>{Player.List.Count(r => r.Role == RoleTypeId.NtfSergeant)}</color></color>\n" +
                    $"<color=yellow>Время до конца: <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>");

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }
            if (Player.List.Count(r => r.Role == RoleTypeId.NtfSergeant) == 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(10, $"<color=red>Зомби Победили!</color>\n" +
                $"<color=yellow>Зомби всех заразили</color>");
            }
            else if (Player.List.Count(r => r.Role == RoleTypeId.Scp0492) == 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(10, $"<color=yellow><color=#D71868><b><i>Люди</i></b></color> Победили!</color>\n" +
                $"<color=yellow>Люди остановили чуму и убили всех зомби</color>");
            }
            else
            {
                Map.ClearBroadcasts();
                Map.Broadcast(10, $"<color=yellow><color=#D71868><b><i>Люди</i></b></color> Победили!</color>\n" +
                $"<color=yellow>Люди выжили, но это ещё не конец</color>");
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
