using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Games.Infection;
using CustomPlayerEffects;
using InventorySystem.Items.Firearms;

namespace AutoEvent.Games.GunGame
{
    public class EventHandler
    {
        Plugin _plugin;

        internal Dictionary<Player, Stats> _playerStats
        {
            get => _plugin.PlayerStats;
            set => _plugin.PlayerStats = value;
        }
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
            _playerStats = plugin.PlayerStats;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            if (!_playerStats.ContainsKey(ev.Player))
            {
                _playerStats.Add(ev.Player, new Stats(0));
            }

            ev.Player.GiveLoadout(_plugin.Config.Loadouts, LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
            // Extensions.SetRole(ev.Player, GunGameRandom.GetRandomRole(), RoleSpawnFlags.None);
            ev.Player.Position = _plugin.SpawnPoints.RandomItem();
            GetWeaponForPlayer(ev.Player);
        }

        [PluginEvent(ServerEventType.PlayerReloadWeapon)]
        public void OnReloading(PlayerReloadWeaponEvent ev)
        {
            SetMaxAmmo(ev.Player);
        }

        [PluginEvent(ServerEventType.PlayerSpawn)]
        public void OnSpawning(PlayerSpawnEvent ev)
        {
            SetMaxAmmo(ev.Player);
        }

        public void OnPlayerDying(PlayerDyingArgs ev)
        {
            ev.IsAllowed = false;

            ev.Target.Health = 100;

            if (_playerStats is null)
            {
                _playerStats = new Dictionary<Player, Stats>();
            }
            if (ev.Attacker != null)
            {
                if (!_playerStats.TryGetValue(ev.Attacker, out Stats statsAttacker))
                {
                    _playerStats.Add(ev.Attacker, new Stats(1));
                    GetWeaponForPlayer(ev.Attacker);
                }
                else
                {
                    statsAttacker.kill++;
                    GetWeaponForPlayer(ev.Attacker);
                }
            }

            if (ev.Target != null)
            {
                ev.Target.Position = _plugin.SpawnPoints.RandomItem();
                GetWeaponForPlayer(ev.Target);
            }
        }

        public void GetWeaponForPlayer(Player player)
        {
            if (player is null)
            {
                DebugLogger.LogDebug("GetWeapon - Player is null");
                return;
            }
            
            if (_plugin.Config.Guns is null)
            {
                GunGameConfig conf = new GunGameConfig();
                _plugin.Config.Guns = conf.Guns;
                DebugLogger.LogDebug("GetWeapon - Gun By Level is null. Setting new Defaults.");
                //return;
            }

            if (_playerStats is null)
            {
                _playerStats = new Dictionary<Player, Stats>();
                // DebugLogger.LogDebug("GetWeapon - Player stats is null");
                // return;
            }
            
            if (!_playerStats.ContainsKey(player))
            {
                _playerStats.Add(player, new Stats(0));
                //DebugLogger.LogDebug("GetWeapon - Player stats doesnt contain player");
                //return;
            }

            if (_playerStats[player] is null)
            {
                _playerStats[player] = new Stats(0);
                //DebugLogger.LogDebug("GetWeapon - Player level is null");
                //return;
            }

            var gun = _plugin.Config.Guns.OrderByDescending(y => y.KillsRequired)
                .FirstOrDefault(x => _playerStats[player].kill >= x.KillsRequired)!.Item;
            if (gun is ItemType.None)
            {
                DebugLogger.LogDebug("GetWeapon - Gun by level is null");
                // return;
                gun = ItemType.GunCOM15;
            }
            
            DebugLogger.LogDebug($"Getting player {player.Nickname} weapon.");
            player.EffectsManager.EnableEffect<SpawnProtected>(.5f);
            player.ClearInventory();
            var item = player.AddItem(gun);
            if (item is Firearm firearm)
            {
                var status = firearm.Status;
                byte ammo = firearm.AmmoManagerModule.MaxAmmo;
                var stats = new FirearmStatus(ammo, status.Flags, status.Attachments);
                firearm.Status = stats;
            }
            Timing.CallDelayed(.1f, () =>
            {
                if (item != null)
                {
                    player.CurrentItem = item;
                }
            });
        }

        private void SetMaxAmmo(Player pl)
        {
            DebugLogger.LogDebug($"Setting max ammo for {pl.Nickname}.");

            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
            {
                pl.SetAmmo(AmmoLimit.Key, AmmoLimit.Value);
            }
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
