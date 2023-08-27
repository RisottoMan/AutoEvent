using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Infection
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = InfectionTranslation.ZombieName;
        public override string Description { get; set; } = InfectionTranslation.ZombieDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Zombie"; //AutoEvent. .ListOfMap.RandomItem()
        public override string CommandName { get; set; } = "zombie";
        public SchematicObject GameMap { get; set; }
        TimeSpan EventTime { get; set; }
        EventHandler _eventHandler { get; set; }

        public override void OnStart()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);

            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;

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
            Players.PlayerDamage -= _eventHandler.OnPlayerDamage;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            MapName = InfectionConfig.ListOfMap.RandomItem();

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

            var music = InfectionConfig.ListOfMusic.ToList().RandomItem();
            Extensions.PlayAudio(music.Key, music.Value, true, Name);

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomPosition.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "zombie_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast(InfectionTranslation.ZombieBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Extensions.SetRole(Player.GetPlayers().RandomItem(), RoleTypeId.Scp0492, RoleSpawnFlags.None);

            while (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 1)
            {
                var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
                var time = $"{EventTime.Minutes}:{EventTime.Seconds}";

                Extensions.Broadcast(InfectionTranslation.ZombieCycle.Replace("{name}", Name).Replace("{count}", count.ToString()).Replace("{time}", time), 1);

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
                if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0) 
                    break;

                Extensions.Broadcast(InfectionTranslation.ZombieExtraTime.Replace("{extratime}", extratime.ToString()).Replace("{time}", time), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(InfectionTranslation.ZombieWin.Replace("{time}", time), 10);
            }
            else
            {
                Extensions.Broadcast(InfectionTranslation.ZombieLose.Replace("{time}", time), 10);
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
