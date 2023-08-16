using AutoEvent.Interfaces;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MapEditorReborn.API.Features.Objects;
using System;
using AutoEvent.Events.Lobby.Features;

namespace AutoEvent.Events.Lobby
{
    public class Plugin// : Event
    {
        public string Name { get; set; } = "Lobby";
        public string Description { get; set; } = "In this lobby, a mini-game is selected and launched.";
        public string Author { get; set; } = "KoT0XleB";
        public string MapName { get; set; } = "Lobby";
        public string CommandName { get; set; } = "lobby";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap;
        EventHandler _eventHandler;
        Player Person;

        public void OnStart()
        {
            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;

            Timing.RunCoroutine(OnEventRunning(), "choice_time");
        }

        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public IEnumerator<float> OnEventRunning()
        {
            EventTime = new TimeSpan(0, 1, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);

            foreach (Player player in Player.List)
            {
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            int time = 15;
            while (time != 0)
            {
                var count = Player.List.Count();
                var text = string.Empty;

                if (count <= 0) // 1
                {
                    Extensions.Broadcast($"Ожидание игроков для запуска Мини-Игр.", 1);
                    time = 15;
                    //yield continue;
                }

                Extensions.Broadcast($"Вы находитесь в лобби\nПриготовьтесь бежать к центру - {time}\nКто успеет - тот выбирает мини-игру.", 1);
                time--;
            }

            List<GameObject> teleports = new List<GameObject>();
            List<GameObject> platformes = new List<GameObject>();
            foreach (var block in GameMap.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "Wall": GameObject.Destroy(block); break;
                    case "Teleport": teleports.Add(block); break;
                    case "Platform": platformes.Add(block); break;
                }
            }

            while (EventTime.TotalSeconds > 0 && Player.List.Count(r => r.IsAlive) > 0)
            {
                foreach(Player player in Player.List)
                {
                    var text = $"Вы находитесь в лобби\n";
                    if (Person != null)
                    {
                        if (player == Person)
                        {
                            text += $"Выберите мини-игру\nУ вас осталось {EventTime.TotalSeconds} секунд.";
                        }
                        else
                        {
                            text += $"Ожидается выбор мини-игры\nУ выбирающего осталось {EventTime.TotalSeconds} секунд.";
                        }
                    }
                    else
                    {
                        text += "Вы должны добраться до центра.";
                    }

                    if (Vector3.Distance(player.Position, teleports.ElementAt(0).transform.position) < 0.1)
                    {
                        Person = player;
                    }

                    player.ClearBroadcasts();
                    player.Broadcast(1, text);
                }

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            //Extensions.StopAudio();
            GameMap.Destroy();
            AutoEvent.ActiveEvent = null;
        }
    }
}
