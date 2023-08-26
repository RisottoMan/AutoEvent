using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Line
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.LineName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.LineDescription;
        public override string Author { get; set; } = "Logic_Gun";
        public override string MapName { get; set; } = "Line";
        public override string CommandName { get; set; } = "line";
        public static SchematicObject GameMap { get; set; }
        public Dictionary<int, SchematicObject> HardGameMap { get; set; }
        public TimeSpan EventTime { get; set; }
        EventHandler _eventHandler { get; set; }
        int HardCounts { get; set; }
        int HardCountsLimit { get; set; } = 8;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;

            OnEventStarted();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;

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

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            }

            Timing.RunCoroutine(OnEventRunning(), "line_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;

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
                Extensions.Broadcast(translation.LineCycle.Replace("%name%", Name).
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
                Extensions.Broadcast(translation.LineMorePlayers.
                    Replace("%name%", Name).
                    Replace("%count%", $"{Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD)}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 1)
            {
                Extensions.Broadcast(translation.LineWinner.
                    Replace("%name%", Name).
                    Replace("%winner%", Player.GetPlayers().First(r => r.Role == RoleTypeId.ClassD).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(translation.LineAllDied, 10);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            foreach (var map in HardGameMap.Values) 
                Extensions.UnLoadMap(map);

            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}