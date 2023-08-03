using AutoEvent.Events.Lava.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Events.Lava
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.LavaName;
        public override string Description { get; set; } = Translation.LavaDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Lava";
        public override string CommandName { get; set; } = "lava";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }

        EventHandler _eventHandler;
        private bool isFreindlyFireEnabled;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

            OnEventStarted();

            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
        }
        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            EventManager.UnregisterEvents(_eventHandler);
            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            GameMap = Extensions.LoadMap(MapName, new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Lava.ogg", 7, false, Name);

            foreach (var player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "lava_time");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(Translation.LavaBeforeStart.Replace("%time%", $"{time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            GameObject lava = GameMap.AttachedBlocks.First(x => x.name == "LavaObject");
            lava.AddComponent<LavaComponent>();

            while (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                string text = string.Empty;
                if (EventTime.TotalSeconds % 2 == 0)
                {
                    text = "<size=90><color=red><b>《 ! 》</b></color></size>\n";
                }
                else
                {
                    text = "<size=90><color=red><b>!</b></color></size>\n";
                }

                Extensions.Broadcast(text + Translation.LavaCycle.Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsAlive)}"), 1);
                lava.transform.position += new Vector3(0, 0.08f, 0);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.LavaWin.Replace("%winner%", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.LavaAllDead, 10);
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
            AutoEvent.ActiveEvent = null;
        }
    }
}
