using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;

namespace AutoEvent.Games.Lava
{
    public class EventHandler
    {
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        private Plugin _plugin;
        public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            // prevent infinite recursion
            if (ev.DamageHandler is CustomReasonDamageHandler)
            {
                ev.IsAllowed = true;
                return;
            }
            if (ev.DamageType == DeathTranslations.Falldown.Id)
            {
                ev.IsAllowed = false;
            }
            
            if (ev.Attacker != null && ev.Target != null)
            {
                ev.IsAllowed = false;
                if (_plugin.Config.GunEffects is null || _plugin.Config.GunEffects.IsEmpty())
                {
                    goto defaultDamage;
                }
                ev.IsAllowed = true;
                _plugin.Config.GunEffects.ApplyWeaponEffect(ref ev);
                ev.Attacker.ReceiveHitMarker(1);
                ev.Target?.Damage(new CustomReasonDamageHandler("Shooting", ev.Amount));
                DebugLogger.LogDebug($"Applying Custom Weapon Effect. Damage: {ev.Amount}");
                ev.IsAllowed = false;
                return;
            }
            defaultDamage:
                ev.Attacker?.ReceiveHitMarker();
                ev.Target?.Damage(new CustomReasonDamageHandler("Shooting", 3f));
                ev.IsAllowed = false;
                return;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Spectator);
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
