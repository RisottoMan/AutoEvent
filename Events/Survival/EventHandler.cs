using MEC;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Map;
using InventorySystem.Configs;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Extensions;
using CustomPlayerEffects;

namespace AutoEvent.Events.Survival
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnDamage(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Player != null)
            {
                if (ev.Attacker.IsScp)
                {
                    ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.AssignInventory);
                    ev.Attacker.ShowHitMarker();
                }
                else if (ev.Attacker.IsHuman)
                {
                    ev.Player.EnableEffect<Stained>();
                    ev.Player.ChangeEffectIntensity<Stained>(0, 1f);
                }
            }
        }

        public void OnDead(DiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
            });
        }

        public void OnJoin(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
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
