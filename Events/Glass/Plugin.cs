using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;
using AutoEvent.Events.Glass.Features;

namespace AutoEvent.Events.Glass
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Прыжок Веры [Testing]";
        public override string Description { get; set; } = "Пропрыгайте в конец карты через препятствия. [Testing]";
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "glass";
        public SchematicObject GameMap { get; set; }
        //public static Model Platformes { get; set; }
        //public static Model ModelCheckPoint { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;

            OnEventStarted();
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            GameMap = Extensions.LoadMap("Glass.json", new Vector3(127.460f, 1016.707f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("FallGuys_BeanThieves.ogg", 10, true, Name);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD);
                //player.Position = GameMap.Position + new Vector3(-28.65f, -3.2f, Random.Range(-1, -13)); // ???

                player.GameObject.AddComponent<BoxCollider>();
                player.GameObject.AddComponent<BoxCollider>().size = new Vector3(1f, 3.5f, 1f);
            }
            Timing.RunCoroutine(OnEventRunning(), "glass_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            while (EventTime.TotalSeconds == 10)
            {
                int count = Player.List.ToList().Count(r => r.Role != RoleTypeId.Spectator);

                // Если все сдохли, то обнуляем;
                if (count <= 0) EventTime = new TimeSpan(0, 0, 0);

                Extensions.Broadcast($"<size=50>Прыжок Веры\nПройдите до конца уровня!</size>\n" +
                    $"<size=20>Игроков: {count} | <color=red>До конца: {EventTime.Minutes}:{EventTime.Seconds} секунд</color></size>", 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            /*
            // Проверка нахождения на примитиве
            foreach (Player player in Player.List.ToList())
            {
                if (Vector3.Distance(ModelCheckPoint.GameObject.transform.position, player.Position) >= 8) // ????
                {
                    player.Hurt(100, "Не успел дойти до финиша.");
                }
            }
            */
            /// Вообще лучше переделать рестарт на один раунд

            if (Player.List.ToList().Count(r => r.Role != RoleTypeId.Spectator) > 1)
            {
                Extensions.Broadcast($"Прыжок Веры\nОсталось много игроков.\nРестарт Ивента!", 5);
                // рестарт мини-игры
                // RestartEvent(); // ???
                yield break;
            }
            else if (Player.List.ToList().Count(r => r.Role.Type != RoleTypeId.Spectator) == 1)
            {
                // Победитель
                Extensions.Broadcast($"Прыжок Веры\n<color=yellow>ПОБЕДИТЕЛЬ </color>", 10); // {Player.List.ToList().First(r => r.Role != RoleType.Spectator).Nickname}
            }
            else if (Player.List.ToList().Count(r => r.Role != RoleTypeId.Spectator) == 0)
            {
                // Все проиграли
                Extensions.Broadcast($"Прыжок Веры\n<color=red>Все погибли)))))))</color>", 10);
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            //GameObject.Destroy(Platformes.GameObject);
            //Platformes.Destroy();
            //ModelCheckPoint.Destroy();
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
