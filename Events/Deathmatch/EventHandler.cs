using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using System.Linq;

namespace AutoEvent.Events.Deathmatch
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }
        public void OnJoin(VerifiedEventArgs ev)
        {
            // Give role
            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) > Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency))
            {
                ev.Player.Role.Set(RoleTypeId.ChaosRifleman);
            }
            else
            {
                ev.Player.Role.Set(RoleTypeId.NtfSergeant);
            }
            ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
            // Effects
            ev.Player.EnableEffect<CustomPlayerEffects.Scp1853>(300);
            ev.Player.EnableEffect(EffectType.MovementBoost, 300);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 25);
            ev.Player.EnableEffect<CustomPlayerEffects.SpawnProtected>(5);
            // Position
            ev.Player.Position = _plugin.GameMap.Position + RandomClass.GetRandomPosition();
        }
        public void OnDying(DyingEventArgs ev)
        {
            // We don't kill the player, but move them around the arena.
            ev.IsAllowed = false;
            // Get exp
            if (ev.Player.Role.Team == Team.FoundationForces)
            {
                _plugin.ChaosKills++;
            }
            else if (ev.Player.Role.Team == Team.ChaosInsurgency)
            {
                _plugin.MtfKills++;
            }
            // Respawn player
            ev.Player.EnableEffect(EffectType.Flashed, 0.1f);
            ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
            ev.Player.Position = _plugin.GameMap.Position + RandomClass.GetRandomPosition();
            ev.Player.EnableEffect<CustomPlayerEffects.SpawnProtected>(1);
            ev.Player.Health = 100;
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
            ev.Player.AddAmmo(AmmoType.Nato9, 100);
            ev.Player.AddAmmo(AmmoType.Ammo44Cal, 100);
            ev.Player.AddAmmo(AmmoType.Nato556, 100);
            ev.Player.AddAmmo(AmmoType.Nato762, 100);
            ev.Player.AddAmmo(AmmoType.Ammo12Gauge, 100);
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
