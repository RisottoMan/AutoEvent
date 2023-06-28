using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;
using Exiled.Events.Commands.Reload;

namespace AutoEvent.Events.Football
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Футбольчик [Testing]";
        public override string Description { get; set; } = "Режим Футбол, в котором надо забить 3 гола противоположной команде. [Testing]";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "football";
        public SchematicObject GameMap { get; set; }
        //public static Model GameMap { get; set; }
        //public static Model Ball { get; set; }
        public int BluePoints { get; set; } = 0;
        public int RedPoints { get; set; } = 0;
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

            GameMap = Extensions.LoadMap("Football.json", new Vector3(127.460f, 1016.707f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one); // спавнпоинты
            Extensions.PlayAudio("FallGuys_DnB.ogg", 5, true, Name);

            // Create ball
            /*
            Ball = new Model("balls", new Vector3(0, 0, 0));
            Ball.AddPart(new ModelPrimitive(Ball, PrimitiveType.Sphere, UnityEngine.Color.red, GameMap.GameObject.transform.position + new Vector3(0, 5f, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
            foreach (var ball in Ball.Primitives)
            {
                ball.GameObject.AddComponent<FootballComponent>();
            }
            */

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                }
                //player.Position = GameMap.GameObject.transform.position + new Vector3(0, 5, 0);
                count++;
            }

            Timing.RunCoroutine(OnEventRunning(), "glass_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            EventTime = new TimeSpan(0, 5, 0);
            BluePoints = 0;
            RedPoints = 0;
            // Запуск
            while (BluePoints < 3 && RedPoints < 3 && EventTime.TotalSeconds > 0)
            {
                foreach (Player player in Player.List)
                {
                    var text = string.Empty;
                    if (player.Role.Type == RoleTypeId.NtfCaptain)
                    {
                        text += "<color=cyan>Вы играете за Синюю Команду</color>\n";
                    }
                    else
                    {
                        text += "<color=red>Вы играете за Красную Команду</color>\n";
                    }
                    /* Переписать код для взаимодействия с мячом
                    // Проверка расстояния между игроком и мячом
                    if (Vector3.Distance(Ball.Primitives[0].Primitive.Position, player.Position) < 5)
                    {
                        Ball.Primitives[0].GameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                        rig.AddForce(player.Transform.forward + new Vector3(0, 0.5f, 0), ForceMode.Impulse);
                    }
                    */
                    Extensions.Broadcast(text + $"<color=blue>{BluePoints}</color> VS <color=red>{RedPoints}</color>\n" +
                        $"Время до конца: {EventTime.Minutes}:{EventTime.Seconds}", 1);
                }
                /*
                // Проверка попадания мяча в синии ворота
                if (Vector3.Distance(Ball.Primitives[0].Primitive.Position, new Vector3(98.47f, 949.87f, -122.48f)) < 5)
                {
                    Ball.Primitives[0].Primitive.Position = GameMap.GameObject.transform.position + new Vector3(0, 5f, 0);
                    RedPoints++;
                }
                // Проверка попадания мяча в красные ворота
                if (Vector3.Distance(Ball.Primitives[0].Primitive.Position, new Vector3(191.71f, 949.87f, -123.13f)) < 5)
                {
                    Ball.Primitives[0].Primitive.Position = GameMap.GameObject.transform.position + new Vector3(0, 5f, 0);
                    BluePoints++;
                }
                */
                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }
            // Голов в синие ворота больше чем красных
            if (BluePoints > RedPoints)
            {
                Extensions.Broadcast($"<color=blue>ПОБЕДА СИНИХ!</color>", 10);
            }
            // Голов красные ворота больше чем синих
            else if (RedPoints > BluePoints)
            {
                Extensions.Broadcast($"<color=red>ПОБЕДА КРАСНЫХ!</color>", 10);
            }
            // Ничья
            else
            {
                Extensions.Broadcast($"<color=#808080>Ничья</color>\n<color=blue>{BluePoints}</color> VS <color=red>{RedPoints}</color>", 10);
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            // Ball.Destroy();
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
