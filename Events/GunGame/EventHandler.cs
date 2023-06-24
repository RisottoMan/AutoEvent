using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Features.Objects;
using System.Collections.Generic;

namespace AutoEvent.Events.GunGame
{
    public class EventHandler
    {
        SchematicObject _gameMap;
        Dictionary<Player, Stats> _playerStats;
        public EventHandler(Plugin plugin)
        {
            _gameMap = plugin.GameMap;
            _playerStats = plugin.PlayerStats;
        }
        public void OnJoin(VerifiedEventArgs ev)
        {
            _playerStats.Add(ev.Player, new Stats
            {
                kill = 0,
                level = 1
            });

            ev.Player.Role.Set(GunGameRandom.GetRandomRole());
            ev.Player.ClearInventory();

            ev.Player.CurrentItem = ev.Player.AddItem(GunGameGuns.GunForLevel[_playerStats[ev.Player].level]);

            ev.Player.Position = _gameMap.Position + GunGameRandom.GetRandomPosition();
        }
        public void OnPlayerDying(DyingEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Player.Health = 100;
            // Attacker shit
            if (ev.Attacker != null)
            {
                _playerStats.TryGetValue(ev.Attacker, out Stats statsAttacker);

                statsAttacker.kill++;
                if (statsAttacker.kill >= 2)
                {
                    statsAttacker.level++;
                    statsAttacker.kill = 0;
                    ev.Attacker.ClearInventory();
                    ev.Attacker.CurrentItem = ev.Attacker.AddItem(GunGameGuns.GunForLevel[_playerStats[ev.Attacker].level]);
                }
            }
            // Target shit
            if (ev.Player != null)
            {
                ev.Player.ClearInventory();

                ev.Player.CurrentItem = ev.Player.AddItem(GunGameGuns.GunForLevel[_playerStats[ev.Player].level]);

                ev.Player.Position = _gameMap.Position + GunGameRandom.GetRandomPosition();
            }
        }
        public void OnReloading(ReloadingWeaponEventArgs ev)
        {
            SetAmmo(ev.Player, 50);
        }
        public void OnSpawned(SpawnedEventArgs ev)
        {
            SetAmmo(ev.Player, 50);
        }
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
        private void SetAmmo(Player pl, ushort Amount)
        {
            pl.SetAmmo(AmmoType.Nato9, Amount);
            pl.SetAmmo(AmmoType.Ammo44Cal, Amount);
            pl.SetAmmo(AmmoType.Nato556, Amount);
            pl.SetAmmo(AmmoType.Nato762, Amount);
            pl.SetAmmo(AmmoType.Ammo12Gauge, Amount);
        }
    }
}
