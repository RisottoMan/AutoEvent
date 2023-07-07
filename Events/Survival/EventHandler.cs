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
using Exiled.API.Enums;
using System.Linq;

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
            if (ev.Player != null && ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.IsAllowed = false;
                return;
            }

            if (ev.Attacker != null && ev.Player != null)
            {
                if (ev.Attacker.IsScp)
                {
                    if (ev.Player.ArtificialHealth <= 50)
                    {
                        ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                        ev.Player.Health = 5000;
                    }
                    else
                    {
                        ev.Amount = 0;
                        ev.Player.ArtificialHealth -= 50;
                    }

                    ev.Attacker.ShowHitMarker();
                }

                if (ev.Attacker.IsHuman && ev.Player.IsScp)
                {
                    ev.Player.EnableEffect<Stained>(1);
                    ev.Player.EnableEffect<Sinkhole>(1);
                }
            }
        }

        public void OnDead(DiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
                ev.Player.EnableEffect<Disabled>();
                ev.Player.EnableEffect<Scp1853>();
                ev.Player.Health = 5000;
            });
        }

        public void OnJoin(VerifiedEventArgs ev)
        {

            if (Player.List.Count(r => r.Role.Type == RoleTypeId.Scp0492) > 0)
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
                ev.Player.EnableEffect<Disabled>();
                ev.Player.EnableEffect<Scp1853>();
                ev.Player.Health = 5000;
            }
            else
            {
                ev.Player.Role.Set(RoleTypeId.NtfSergeant, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
                ev.Player.AddAhp(100, 100, 0, 0, 0, true);

                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
                });
            }
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
