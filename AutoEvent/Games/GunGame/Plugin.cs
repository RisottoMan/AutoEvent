using MER.Lite.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using UnityEngine;
using Utils.NonAllocLINQ;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.GunGame
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GunGameTranslate.GunGameName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GunGameTranslate.GunGameDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.GunGameTranslate.GunGameCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);

        [EventConfig]
        public GunGameConfig Config { get; set; }

        [EventConfigPreset] public GunGameConfig EasyGunsFirst => GunGameConfigPresets.EasyGunsFirst;
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Shipment", Position = new Vector3(93f, 1020f, -43f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { SoundName = "Ultrakill.ogg", Volume = 10, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private GunGameTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.GunGameTranslate;
        internal List<Vector3> SpawnPoints { get; private set; }
        internal Dictionary<Player, Stats> PlayerStats { get; set; }
        private Player _winner;

        protected override void RegisterEvents()
        {
            PlayerStats = new Dictionary<Player, Stats>();

            EventHandler = new EventHandler(this);

            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
            Players.PlayerDying += EventHandler.OnPlayerDying;
        }

        protected override void UnregisterEvents()
        {
            Server.FriendlyFire = false;

            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropItem -= EventHandler.OnDropItem;
            Players.DropAmmo -= EventHandler.OnDropAmmo;
            Players.PlayerDying -= EventHandler.OnPlayerDying;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            Server.FriendlyFire = false;

            _winner = null;
            SpawnPoints = new List<Vector3>();

            foreach(var point in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Spawnpoint"))
            {
                SpawnPoints.Add(point.transform.position);
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (!PlayerStats.ContainsKey(player))
                {
                    PlayerStats.Add(player, new Stats(0));
                }

                player.ClearInventory();
                //Extensions.SetRole(player, GunGameRandom.GetRandomRole(), RoleSpawnFlags.None);
                player.GiveLoadout(Config.Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                player.Position = SpawnPoints.RandomItem();

                count++;
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            Server.FriendlyFire = true;

            foreach (var player in Player.GetPlayers())
            {
                if (player is not null)
                {
                    EventHandler.GetWeaponForPlayer(player);
                }
            }
        }

        protected override bool IsRoundDone()
        {
            // Winner is not null &&
            // Over one player is alive && 
            // Elapsed time is smaller than 10 minutes (+ countdown)
            return !(_winner == null && Enumerable.Count(Player.GetPlayers(), r => r.IsAlive) > 1 && EventTime.TotalSeconds < 600);
        }
        
        protected override void ProcessFrame()
        {
            var leaderStat = PlayerStats.OrderByDescending(r => r.Value.kill).FirstOrDefault();
            List<GunRole> gunsInOrder = Config.Guns.OrderByDescending(x => x.KillsRequired).ToList();
            GunRole leadersWeapon = gunsInOrder.FirstOrDefault(x => leaderStat.Value.kill >= x.KillsRequired);
            foreach (Player pl in Player.GetPlayers())
            {
                PlayerStats.TryGetValue(pl, out Stats stats);
                if (stats.kill >= Config.Guns.OrderByDescending(x => x.KillsRequired).FirstOrDefault()!.KillsRequired)
                {
                    _winner = pl;
                }

                int kills = EventHandler._playerStats[pl].kill;
                ListExtensions.TryGetFirstIndex(gunsInOrder, x => kills >= x.KillsRequired, out int indexOfFirst);

                string nextGun = "";
                int killsNeeded = 0; 
                if (indexOfFirst <= 0)
                {
                    killsNeeded = gunsInOrder[0].KillsRequired + 1 - kills;
                    nextGun = "Last Weapon";
                }
                else
                {
                    /*
                     * 0 Most Kill Gun
                       1 Medium Kill Gun
                       2 Current gun - get current gun - 1 for next
                       3 Lowest kill gun
                    */
                    killsNeeded = gunsInOrder[indexOfFirst - 1].KillsRequired - kills;
                    nextGun = gunsInOrder[indexOfFirst - 1].Item.ToString();
                }
                pl.ClearBroadcasts();
                pl.SendBroadcast(
                    Translation.GunGameCycle.Replace("{name}", Name).Replace("{gun}", nextGun )
                        .Replace("{kills}", $"{killsNeeded}").Replace("{leadnick}", leaderStat.Key.Nickname)
                        .Replace("{leadgun}", $"{(leadersWeapon is null ? nextGun : leadersWeapon.Item)}"), 1);
            }
        }

        protected override void OnFinished()
        {
            if (_winner != null)
            {
                Extensions.Broadcast(
                    Translation.GunGameWinner.Replace("{name}", Name).Replace("{winner}", _winner.Nickname), 10);
            }

            foreach (var player in Player.GetPlayers())
            {
                player.ClearInventory();
            }
        }

    }
}
