using MEC;
using PlayerRoles;
using InventorySystem.Configs;
using System.Collections.Generic;
using CustomPlayerEffects;
using System.Linq;
using AutoEvent.Events.EventArgs;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core;
using PlayerStatsSystem;

namespace AutoEvent.Games.ZombieEscape
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            if (ev.DamageType == DeathTranslations.Falldown.Id)
            {
                ev.IsAllowed = false;
            }

            if (ev.Attacker != null && ev.Target != null)
            {
                if (ev.Attacker.IsSCP)
                {
                    if (ev.Target.ArtificialHealth <= 50)
                    {
                        ev.Target.SetRole(RoleTypeId.Scp0492);
                        ev.Target.Health = 5000;
                    }
                    else
                    {
                        ev.Amount = 0;
                        ev.Target.ArtificialHealth -= 50;
                    }

                    ev.Attacker.ReceiveHitMarker();
                }

                if (ev.Attacker.IsHuman && ev.Target.IsSCP)
                {
                    ev.Target.EffectsManager.EnableEffect<Stained>(1);
                    ev.Target.EffectsManager.EnableEffect<Sinkhole>(1);
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scp0492) > 0)
            {
                Extensions.SetRole(ev.Player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
                ev.Player.EffectsManager.EnableEffect<Disabled>();
                ev.Player.EffectsManager.EnableEffect<Scp1853>();
                ev.Player.Health = 10000;
            }
            else
            {
                Extensions.SetRole(ev.Player, RoleTypeId.NtfSergeant, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
                ev.Player.ArtificialHealth = 100;

                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
                });
            }
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

        private void SetMaxAmmo(Player pl)
        {
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
