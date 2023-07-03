using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.GunGame
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GunGameName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GunGameDescription;
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "gungame";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public List<Vector3> Spawners { get; set; } = new List<Vector3>();
        public Player Winner { get; set; }
        public Dictionary<Player, Stats> PlayerStats = new Dictionary<Player, Stats>();

        private bool isFreindlyFireEnabled;
        EventHandler _eventHandler;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

            OnEventStarted();
            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Dying += _eventHandler.OnPlayerDying;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned += _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon += _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;
        }
        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnPlayerDying;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Winner = null;
            GameMap = Extensions.LoadMap("Shipment", new Vector3(5f, 1030f, -45f), Quaternion.Euler(Vector3.zero), Vector3.one); // new Vector3(120f, 1020f, -43.5f)
            Extensions.PlayAudio("ClassicMusic.ogg", 3, true, Name);

            var count = 0;
            foreach (Player player in Player.List)
            {
                count++;

                PlayerStats.Add(player, new Stats()
                {
                    kill = 0,
                    level = 1
                });

                player.Role.Set(GunGameRandom.GetRandomRole(), SpawnReason.None, RoleSpawnFlags.None);
                player.Position = GunGameRandom.GetRandomPosition(GameMap);
                player.EnableEffect<CustomPlayerEffects.SpawnProtected>(10);

                var item = player.AddItem(GunGameGuns.GunForLevel[PlayerStats[player].level]);
                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = item;
                });
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

            AutoEvent.ActiveEvent = null;
        }
    }
}
