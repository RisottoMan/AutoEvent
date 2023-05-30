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
            ev.Player.CurrentItem = Item.Create(GunGameGuns.GunForLevel[_playerStats[ev.Player].level], ev.Player);
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
                    ev.Attacker.CurrentItem = Item.Create(GunGameGuns.GunForLevel[_playerStats[ev.Attacker].level], ev.Attacker);
                }
            }
            // Target shit
            if (ev.Player != null)
            {
                ev.Player.ClearInventory();
                ev.Player.CurrentItem = Item.Create(GunGameGuns.GunForLevel[_playerStats[ev.Player].level], ev.Player);
                ev.Player.EnableEffect<CustomPlayerEffects.SpawnProtected>(1);
                ev.Player.Position = _gameMap.Position + GunGameRandom.GetRandomPosition();
            }
        }
        public void OnShooting(ShootingEventArgs ev)
        {
            switch (ev.Player.Inventory.CurItem.TypeId)
            {
                case (ItemType.GunCOM15):
                case (ItemType.GunCOM18):
                case (ItemType.GunCrossvec):
                case (ItemType.GunFSP9): { ev.Player.AddAmmo(AmmoType.Nato9, 1); } break;
                case (ItemType.GunRevolver): { ev.Player.AddAmmo(AmmoType.Ammo44Cal, 1); } break;
                case (ItemType.GunE11SR): { ev.Player.AddAmmo(AmmoType.Nato556, 1); } break;
                case (ItemType.GunAK):
                case (ItemType.GunLogicer): { ev.Player.AddAmmo(AmmoType.Nato762, 1); } break;
                case (ItemType.GunShotgun): { ev.Player.AddAmmo(AmmoType.Ammo12Gauge, 1); } break;
            }
        }
        public void OnSpawned(SpawnedEventArgs ev)
        {
            ev.Player.AddAmmo(AmmoType.Nato9, 50);
            ev.Player.AddAmmo(AmmoType.Ammo44Cal, 50);
            ev.Player.AddAmmo(AmmoType.Nato556, 50);
            ev.Player.AddAmmo(AmmoType.Nato762, 50);
            ev.Player.AddAmmo(AmmoType.Ammo12Gauge, 50);
        }
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
