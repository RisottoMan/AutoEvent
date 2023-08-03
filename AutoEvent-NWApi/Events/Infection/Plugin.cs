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

namespace AutoEvent.Events.Infection
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.ZombieName;
        public override string Description { get; set; } = Translation.ZombieDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = InfectionConfig.ListOfMap.RandomItem();
        public override string CommandName { get; set; } = "zombie";
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
            EventTime = new TimeSpan(0, 0, 0);

            float scale = 1;
            switch(Player.GetPlayers().Count())
            {
                case int n when (n > 15 && n <= 20): scale = 1.1f; break;
                case int n when (n > 20 && n <= 25): scale = 1.2f; break;
                case int n when (n > 25 && n <= 30): scale = 1.3f; break;
                case int n when (n > 30 && n <= 35): scale = 1.4f; break;
                case int n when (n > 35): scale = 1.5f; break;
            }

            GameMap = Extensions.LoadMap(MapName, new Vector3(115.5f, 1030f, -43.5f), Quaternion.identity, new Vector3(1, 1, 1) * scale);
            Extensions.PlayAudio(InfectionConfig.ListOfMusic.RandomItem(), 7, true, Name);

            foreach (Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "zombie_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.ZombieBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Player.GetPlayers().RandomItem().SetRole(RoleTypeId.Scp0492);

            while (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 1)
            {
                var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
                var time = $"{EventTime.Minutes}:{EventTime.Seconds}";

                Extensions.Broadcast(Translation.ZombieCycle.Replace("{name}", Name).Replace("{count}", count.ToString()).Replace("{time}", time), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            Timing.RunCoroutine(DopTime(), "EventBeginning");
            yield break;
        }

        public IEnumerator<float> DopTime()
        {
            var time = $"{EventTime.Minutes}:{EventTime.Seconds}";

            for (int extratime = 30; extratime > 0; extratime--)
            {
                if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0) break;
                Extensions.Broadcast(Translation.ZombieExtraTime.Replace("{extratime}", extratime.ToString()).Replace("{time}", time), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(Translation.ZombieWin.Replace("{time}", time), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.ZombieLose.Replace("{time}", time), 10);
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

            MapName = InfectionConfig.ListOfMap.RandomItem();
            AutoEvent.ActiveEvent = null;
        }
    }
}
