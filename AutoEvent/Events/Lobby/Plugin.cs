using AutoEvent.Interfaces;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MapEditorReborn.API.Features.Objects;
using System;

namespace AutoEvent.Events.Lobby
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Lobby Choice";
        public override string Description { get; set; } = "In this lobby, a mini-game is selected and launched.";
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Lobby";
        public override string CommandName { get; set; } = "lobby";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap;
        EventHandler _eventHandler;
        Player Person;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            //Exiled.Events.Handlers.Player.Verified += _eventHandler.OnPlayerVerified;

            Timing.RunCoroutine(OnEventRunning(), "choice_time");
        }
        public override void OnStop()
        {
            //Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnPlayerVerified;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public IEnumerator<float> OnEventRunning()
        {
            EventTime = new TimeSpan(0, 1, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);

            for (int time = 30; time > 0; time--)
            {
                Extensions.Broadcast($"Вы находитесь в лобби\nПриготовьтесь бежать к центру - {time}\nКто успеет - тот выбирает мини-игру.", 1);
                yield return Timing.WaitForSeconds(1f);
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
            Extensions.StopAudio();
            GameMap.Destroy();
            AutoEvent.ActiveEvent = null;
        }
    }
}
