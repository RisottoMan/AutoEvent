using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Configs;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System.Collections.Generic;

namespace AutoEvent.Events.GunGame
{
    public class EventHandler
    {
        Plugin _plugin;
        SchematicObject _gameMap;
        Dictionary<Player, Stats> _playerStats;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
            _gameMap = plugin.GameMap;
            _playerStats = plugin.PlayerStats;
        }
        
        public void OnJoin(VerifiedEventArgs ev)
        {
            if (!_playerStats.ContainsKey(ev.Player))
            {
                _playerStats.Add(ev.Player, new Stats { level = 1, kill = 0 });
            }

            ev.Player.Role.Set(GunGameRandom.GetRandomRole(), SpawnReason.None , RoleSpawnFlags.None);
            ev.Player.Position = _plugin.SpawnPoints.RandomItem();
            GetWeaponForPlayer(ev.Player);
        }

        public void OnPlayerDying(DyingEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Player.Health = 100;

            if (ev.Attacker != null)
            {
                _playerStats.TryGetValue(ev.Attacker, out Stats statsAttacker);

                statsAttacker.kill++;

                if (statsAttacker.kill >= 2)
                {
                    statsAttacker.kill = 0;
                    statsAttacker.level++;
                    GetWeaponForPlayer(ev.Attacker);
                }
            }

            if (ev.Player != null)
            {
                ev.Player.Position = _plugin.SpawnPoints.RandomItem();
                GetWeaponForPlayer(ev.Player);
            }
        }

        public void GetWeaponForPlayer(Player player)
        {
            player.IsGodModeEnabled = true;
            player.ClearInventory();
            var item = player.AddItem(GunGameGuns.GunByLevel[_playerStats[player].level]);

            Timing.CallDelayed(0.1f, () =>
            {
                player.CurrentItem = item;
                player.IsGodModeEnabled = false;
            });
        }

        public void OnReloading(ReloadingWeaponEventArgs ev)
        {
            SetMaxAmmo(ev.Player);
        }

        public void OnSpawned(SpawnedEventArgs ev)
        {
            SetMaxAmmo(ev.Player);
        }

        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
        private void SetMaxAmmo(Player pl)
        {
            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
                pl.SetAmmo(AmmoLimit.Key.GetAmmoType(), AmmoLimit.Value);
        }
    }
}
