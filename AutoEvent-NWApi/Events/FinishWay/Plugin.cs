using AutoEvent.Events.FinishWay.Features;
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

namespace AutoEvent.Events.FinishWay
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.FinishWayName;
        public override string Description { get; set; } = Translation.FinishWayDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "FinishWay";
        public override string CommandName { get; set; } = "finish";
        public static SchematicObject GameMap { get; set; }
        public static TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            OnEventStarted();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            GameMap = Extensions.LoadMap(MapName, new Vector3(115.5f, 1030f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);

            foreach (Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                player.Position = RandomPosition.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "finish_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (float time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<b>{time}</b>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Extensions.PlayAudio("FinishWay.ogg", 8, false, "Finish");

            var point = new GameObject();
            foreach(var gameObject in GameMap.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Wall": { GameObject.Destroy(gameObject); } break;
                    case "Lava": { gameObject.AddComponent<LavaComponent>(); } break;
                    case "FinishTrigger": { point = gameObject; } break;
                }
            }

            EventTime = new TimeSpan(0, 1, 0);
            while (Player.GetPlayers().Count(r => r.IsAlive) > 0 && EventTime.TotalSeconds > 0)
            {
                var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
                var time = $"{EventTime.Minutes}:{EventTime.Seconds}";

                Extensions.Broadcast(Translation.FinishWayCycle.Replace("%name%", Name).Replace("%time%", time), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            foreach(Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, point.transform.position) > 10)
                {
                    player.Kill(Translation.FinishWayDied);
                }
            }

            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(Translation.FinishWaySeveralSurvivors.Replace("%count%", Player.GetPlayers().Count(r => r.IsAlive).ToString()), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.FinishWayOneSurvived.Replace("%player%", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.FinishWayNoSurvivors, 10);
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
