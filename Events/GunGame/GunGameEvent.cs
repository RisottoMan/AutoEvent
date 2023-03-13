using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AutoEvent.Events
{
    internal class GunGameEvent : IEvent
    {
        public string Name => AutoEvent.Singleton.Translation.GunGameName;
        public string Description => AutoEvent.Singleton.Translation.GunGameDescription;
        public string Color => "FFFF00";
        public string CommandName => "gungame";
        public TimeSpan EventTime { get; set; }
        public static SchematicObject GameMap { get; set; }
        public static List<Vector3> Spawners { get; set; } = new List<Vector3>();
        public static Player Winner { get; set; }
        public static Dictionary<Player, Stats> PlayerStats;
        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += GunGameHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += GunGameHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Dying += GunGameHandler.OnPlayerDying;
            Exiled.Events.Handlers.Player.SpawningRagdoll += GunGameHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned += GunGameHandler.OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += GunGameHandler.OnDropItem;
            Exiled.Events.Handlers.Map.PlacingBulletHole += GunGameHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += GunGameHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.Shooting += GunGameHandler.OnShooting;
            Exiled.Events.Handlers.Player.DroppingAmmo += GunGameHandler.OnDropAmmo;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= GunGameHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= GunGameHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Dying -= GunGameHandler.OnPlayerDying;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= GunGameHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= GunGameHandler.OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem -= GunGameHandler.OnDropItem;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= GunGameHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= GunGameHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.Shooting -= GunGameHandler.OnShooting;
            Exiled.Events.Handlers.Player.DroppingAmmo -= GunGameHandler.OnDropAmmo;

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Winner = null;
            PlayerStats = new Dictionary<Player, Stats>();
            GameMap = Extensions.LoadMap("Shipment", new Vector3(120f, 1020f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            Extensions.PlayAudio("ClassicMusic.ogg", 3, true, Name);

            var count = 0;
            foreach (Player player in Player.List)
            {
                PlayerStats.Add(player, new Stats
                {
                    kill = 0,
                    level = 1
                });

                player.Role.Set(GunGameRandom.GetRandomRole());
                player.ClearInventory();
                player.CurrentItem = Item.Create(GunGameGuns.GunForLevel[PlayerStats[player].level], player);
                player.Position = GameMap.Position + GunGameRandom.GetRandomPosition();
                player.EnableEffect<CustomPlayerEffects.SpawnProtected>(10);

                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "gungame_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
            Server.FriendlyFire = true;

            // If you need to stop the game, then just kill all the players
            while (Winner == null && Player.List.Count(r => r.IsAlive) > 0)
            {
                foreach (Player pl in Player.List)
                {
                    PlayerStats.TryGetValue(pl, out Stats stats);
                    if (stats.level == GunGameGuns.GunForLevel.Last().Key)
                    {
                        Winner = pl;
                    }
                    pl.ClearBroadcasts();
                    pl.Broadcast(1, trans.GunGameCycle.Replace("{name}", Name).Replace("{level}", $"{stats.level}").Replace("{kills}", $"{2 - stats.kill}"));
                }
                yield return Timing.WaitForSeconds(1f);
            }
            if (Winner != null)
            {
                Extensions.Broadcast(trans.GunGameWinner.Replace("{name}", Name).Replace("{winner}", Winner.Nickname), 10);
            }
            foreach(Player pl in Player.List)
            {
                pl.ClearInventory();
            }
            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Server.FriendlyFire = false;
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
