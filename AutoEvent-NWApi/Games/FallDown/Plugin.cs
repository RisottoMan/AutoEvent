using AutoEvent.API.Schematic.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Handlers;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.FallDown
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.FallTranslate.FallName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.FallTranslate.FallDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "FallDown";
        public override string CommandName { get; set; } = "fall";
        public static SchematicObject GameMap { get; set; }
        TimeSpan EventTime { get; set; }
        EventHandler _eventHandler { get; set; }
        GameObject Lava { get; set; }

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
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(10f, 1020f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Puzzle.ogg", 15, true, Name);

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomPosition.GetSpawnPosition(GameMap);
            }

            Lava = GameMap.AttachedBlocks.First(x => x.name == "Lava");
            Lava.AddComponent<LavaComponent>();

            Timing.RunCoroutine(OnEventRunning(), "fall_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation.FallTranslate;

            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            List<GameObject> platformes = GameMap.AttachedBlocks.Where(x => x.name == "Platform").ToList();
            GameObject.Destroy(GameMap.AttachedBlocks.First(x => x.name == "Wall"));

            while (Player.GetPlayers().Count(r => r.IsAlive) > 1 && platformes.Count > 1)
            {
                var count = Player.GetPlayers().Count(r => r.IsAlive);
                var time = $"{EventTime.Minutes}:{EventTime.Seconds}";
                Extensions.Broadcast(translation.FallBroadcast.Replace("%name%", Name).Replace("%time%", time).Replace("%count%", $"{count}"), 1);

                var platform = platformes.RandomItem();
                platformes.Remove(platform);
                GameObject.Destroy(platform);

                yield return Timing.WaitForSeconds(0.9f);
                EventTime += TimeSpan.FromSeconds(0.9f);
            }

            if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(translation.FallWinner.Replace("%winner%", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(translation.FallDied, 10);
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
