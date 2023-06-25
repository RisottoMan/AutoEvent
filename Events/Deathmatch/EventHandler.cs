using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Configs;
using PlayerRoles;
using System.Collections.Generic;
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
            ev.Player.Health = 100;
        }
        public void OnReloading(ReloadingWeaponEventArgs ev)
        {
            SetMaxAmmo(ev.Player);
        }
        public void OnSpawned(SpawnedEventArgs ev)
        {
            SetMaxAmmo(ev.Player);
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
        private void SetMaxAmmo(Player pl)
        {
            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
                pl.SetAmmo(AmmoLimit.Key.GetAmmoType(), AmmoLimit.Value);
        }
    }
}
