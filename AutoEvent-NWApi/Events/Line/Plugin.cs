using AutoEvent.Events.Line.Features;
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

namespace AutoEvent.Events.Line
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.LineName;
        public override string Description { get; set; } = Translation.LineDescription;
        public override string Author { get; set; } = "Logic_Gun";
        public override string MapName { get; set; } = "Line";
        public override string CommandName { get; set; } = "line";
        public static SchematicObject GameMap { get; set; }
        public Dictionary<int, SchematicObject> HardGameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        private int HardCounts;
        private int HardCountsLimit = 8;

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
            HardGameMap = new Dictionary<int, SchematicObject>();
            HardCounts = 0;
            EventTime = TimeSpan.FromMinutes(2f);

            GameMap = Extensions.LoadMap(MapName, new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);

            Extensions.PlayAudio("LineLite.ogg", 10, true, Name);

            Player.GetPlayers().ToList().ForEach(pl =>
            {
                pl.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                pl.Position = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            });
            Timing.RunCoroutine(OnEventRunning(), "line_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            foreach (var block in GameMap.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "DeadZone": block.AddComponent<LineComponent>(); break;
                    case "DeadWall": block.AddComponent<LineComponent>(); break;
                    case "Line": block.AddComponent<LineComponent>(); break;
                    case "Shield": GameObject.Destroy(block); break;
                }
            }

            while (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 1 && EventTime.TotalSeconds > 0)
            {
                Extensions.Broadcast(Translation.LineCycle.Replace("%name%", Name).
                    Replace("%min%", $"{EventTime.Minutes}").
                    Replace("%sec%", $"{EventTime.Seconds}").
                    Replace("%count%", $"{Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD)}"), 10);

                if (EventTime.Seconds == 30 && HardCounts < HardCountsLimit)
                {
                    if (HardCounts == 0)
                    {
                        Extensions.StopAudio();
                        Extensions.PlayAudio("LineHard.ogg", 10, true, Name);
                    }

                    try
                    {
                        var map_hard = Extensions.LoadMap("HardLine", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
                        HardGameMap.Add(HardCounts, map_hard);
                    }
                    catch(Exception ex)
                    {
                        Log.Info($"{ex}");
                    }
                    HardCounts++;
                }

                EventTime -= TimeSpan.FromSeconds(1f);
                yield return Timing.WaitForSeconds(1f);
            }    

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 1)
            {
                Extensions.Broadcast(Translation.LineMorePlayers.
                    Replace("%name%", Name).
                    Replace("%count%", $"{Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD)}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 1)
            {
                Extensions.Broadcast(Translation.LineWinner.
                    Replace("%name%", Name).
                    Replace("%winner%", Player.GetPlayers().First(r => r.Role == RoleTypeId.ClassD).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.LineAllDied, 10);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            foreach (var map in HardGameMap.Values) Extensions.UnLoadMap(map);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}