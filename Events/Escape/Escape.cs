using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Cassie;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using InventorySystem.Items.Pickups;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AutoEvent.Events
{
    internal class EscapeEvent : IEvent
    {
        public string Name => "Атомный Побег";
        public string Description => "Сбегите с комплекса Печеньками на сверхзвуковой скорости!";
        public string Color => "FFFF00";
        public string CommandName => "escape";
        public static TimeSpan EventTime { get; set; }
        public int Votes { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Warhead.Stopping += EscapeHandler.OnNukeDisable;
            Exiled.Events.Handlers.Player.Verified += EscapeHandler.OnJoin;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage += EscapeHandler.OnSendCassie;
            Exiled.Events.Handlers.Server.RespawningTeam += EscapeHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Warhead.Stopping -= EscapeHandler.OnNukeDisable;
            Exiled.Events.Handlers.Player.Verified -= EscapeHandler.OnJoin;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage -= EscapeHandler.OnSendCassie;
            Exiled.Events.Handlers.Server.RespawningTeam -= EscapeHandler.OnTeamRespawn;
            Timing.CallDelayed(5f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            // Делаем всех д классами
            Player.List.ToList().ForEach(player =>
            {
                player.Role.Set(RoleTypeId.Scp173, SpawnReason.None, PlayerRoles.RoleSpawnFlags.All);
                player.EnableEffect(EffectType.Ensnared);
            });

            Extensions.PlayAudio("Escape.ogg", 25, true, "Побег ДЦП");

            // Запуск боеголовки
            Warhead.Start();
            Warhead.DetonationTimer = 80f;
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
            Player.List.ToList().ForEach(player => player.DisableEffect(EffectType.Ensnared));
            // Отсчет времени
            while (Warhead.DetonationTimer != 0)
            {

                foreach (Player player in Player.List)
                {
                    if (player.CurrentRoom.Name.ToLower() == "ez_gatea" || player.CurrentRoom.Name.ToLower() == "ez_gateb") 
                        player.Position = new Vector3(0, 1002, 0);
                }

                Map.ClearBroadcasts();
                Map.Broadcast(new Exiled.API.Features.Broadcast($"Атомный Побег\n" +
                    $"До взрыва: <color=red>{(int)Warhead.DetonationTimer}</color> секунд", 1));
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
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
