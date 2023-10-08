using AutoEvent.Events.EventArgs;
using AutoEvent.Interfaces;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine.Assertions.Must;

namespace AutoEvent.Games.DeathParty
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnPlayerDying(PlayerDyingArgs ev)
        {
            Timing.CallDelayed(1f, () =>
            {
                ev.Target.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Respawn);
                ev.Target.Position = RandomClass.GetSpawnPosition(_plugin.MapInfo.Map);
                ev.Target.ClearInventory();
                var item = ev.Target.AddItem(ItemType.GrenadeHE);
                Timing.CallDelayed(.1f, () =>
                {
                    ev.Target.CurrentItem = item;
                });
                ev.Target.ReceiveHint("You have a grenade! Throw it at the people who are still alive!", 5f);
                ev.Target.IsGodModeEnabled = true;
            });
        }

        [PluginEvent(ServerEventType.PlayerThrowProjectile)]
        public void OnPlayerThrowing(Player ply, ThrowableItem throwableItemitem, ThrowableItem.ProjectileSettings projectileSettings, bool isAllowed)
        {
            //MyPlayer plr, ItemBase item, Rigidbody rb
            if (ply.Role != RoleTypeId.ChaosConscript)
            {
                return;
            }
            Timing.CallDelayed(3f, () =>
            {
                var item = ply.AddItem(ItemType.GrenadeHE);
                Timing.CallDelayed(.1f, () =>
                {
                    ply.CurrentItem = item;
                });
            });
        }
        
        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Spectator);
        }

        public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            if (ev.AttackerHandler is ExplosionDamageHandler)
            {
                ev.IsAllowed = false;

                if (_plugin.Stage != 5)
                {
                    ev.Target.Damage(10, "Grenade");
                }
                else
                {
                    ev.Target.Damage(100, "Grenade");
                }
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