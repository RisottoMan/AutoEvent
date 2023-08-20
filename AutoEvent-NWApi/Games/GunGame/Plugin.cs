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

namespace AutoEvent.Games.GunGame
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GunGameName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GunGameDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Shipment";
        public override string CommandName { get; set; } = "gungame";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public List<Vector3> Spawners { get; set; } = new List<Vector3>();
        public Player Winner { get; set; }
        public Dictionary<Player, Stats> PlayerStats;
        public List<Vector3> SpawnPoints;

        private bool isFreindlyFireEnabled;
        EventHandler _eventHandler;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;
            OnEventStarted();

            _eventHandler = new EventHandler(this);

            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDying += _eventHandler.OnPlayerDying;
        }
        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PlayerDying -= _eventHandler.OnPlayerDying;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 10, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(93f, 1020f, -43f), Quaternion.identity, Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 3, true, Name);

            Winner = null;
            PlayerStats = new Dictionary<Player, Stats>();
            SpawnPoints = new List<Vector3>();

            foreach(var point in GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint"))
            {
                SpawnPoints.Add(point.transform.position);
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (!PlayerStats.ContainsKey(player))
                {
                    PlayerStats.Add(player, new Stats() { level = 1, kill = 0 });
                }

                player.ClearInventory();
                Extensions.SetRole(player, GunGameRandom.GetRandomRole(), RoleSpawnFlags.None);
                player.Position = SpawnPoints.RandomItem();

                count++;
            }

            Timing.RunCoroutine(OnEventRunning(), "gungame_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Server.FriendlyFire = true;

            foreach (var player in Player.GetPlayers())
            {
                _eventHandler.GetWeaponForPlayer(player);
            }

            while (Winner == null && Player.GetPlayers().Count(r => r.IsAlive) > 1 && EventTime.TotalSeconds > 0)
            {
                var leaderStat = PlayerStats.OrderByDescending(r => r.Value.level).FirstOrDefault();

                foreach (Player pl in Player.GetPlayers())
                {
                    PlayerStats.TryGetValue(pl, out Stats stats);
                    if (stats.level == GunGameGuns.GunByLevel.Last().Key)
                    {
                        Winner = pl;
                    }

                    pl.ClearBroadcasts();
                    pl.SendBroadcast(translation.GunGameCycle.Replace("{name}", Name).
                        Replace("{level}", $"{stats.level}").
                        Replace("{kills}", $"{2 - stats.kill}").
                        Replace("{leadnick}", leaderStat.Key.Nickname).
                        Replace("{leadlevel}", $"{leaderStat.Value.level}"), 1);
                }
                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            if (Winner != null)
            {
                Extensions.Broadcast(translation.GunGameWinner.Replace("{name}", Name).Replace("{winner}", Winner.Nickname), 10);
            }

            foreach(var player in Player.GetPlayers())
            {
                player.ClearInventory();
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
