using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Events
{
    internal class EscapeEvent : IEvent
    {
        public string Name => "Атомный Побег";
        public string Description => "Сбегите с комплекса Печеньками на сверхзвуковой скорости!";
        public string Color => "FFFF00";
        public string CommandName => "escape";
        public static TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += EscapeHandler.OnJoin;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage += EscapeHandler.OnSendCassie;
            Exiled.Events.Handlers.Server.RespawningTeam += EscapeHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= EscapeHandler.OnJoin;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage -= EscapeHandler.OnSendCassie;
            Exiled.Events.Handlers.Server.RespawningTeam -= EscapeHandler.OnTeamRespawn;
            Timing.CallDelayed(5f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }

        public void OnEventStarted()
        {
            // Делаем всех д классами
            Player.List.ToList().ForEach(player =>
            {
                player.Role.Set(RoleTypeId.Scp173, SpawnReason.None, RoleSpawnFlags.All);
                player.EnableEffect(EffectType.Ensnared);
            });
            // we need Running in the 90's and Vicky Vale - Dancing lmao :D
            Extensions.PlayAudio("Escape.ogg", 25, true, "Побег ДЦП");

            // Запуск боеголовки
            Warhead.DetonationTimer = 120f;
            Warhead.Start();
            Warhead.IsLocked = true;

            Timing.RunCoroutine(OnEventRunning(), "escape_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            // Обнуление таймера
            EventTime = new TimeSpan(0, 0, 0);
            // Отсчет обратного времени
            for (int time = 10; time > 0; time--)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(new Exiled.API.Features.Broadcast($"Атомный Побег\n" +
                    $"Успейте сбежать с комплекса пока он не взоврался!\n" +
                    $"<color=red>До начала побега: {(int)time} секунд</color>", 1));
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
            // Выключаем остановку
            Player.List.ToList().ForEach(player => player.DisableAllEffects());
            var explosionTime = 80;
            // Отсчет времени
            while (EventTime.TotalSeconds != explosionTime)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(new Exiled.API.Features.Broadcast($"Атомный Побег\n" +
                    $"До взрыва: <color=red>{explosionTime - EventTime.TotalSeconds}</color> секунд", 1));
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            Warhead.IsLocked = false;
            Warhead.Stop();

            foreach(Player player in Player.List)
            {
                player.EnableEffect<CustomPlayerEffects.Flashed>(1);
                if (player.Position.y < 990f)
                {
                    player.Kill(DamageType.Warhead);
                }
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Map.ClearBroadcasts();
            Map.Broadcast(new Exiled.API.Features.Broadcast($"Атомный Побег\n" +
                $"<color=red>ПОБЕДА SCP</color>", 10));
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
        }
    }
}
