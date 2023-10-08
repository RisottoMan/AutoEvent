using MEC;
using PlayerRoles;
using InventorySystem.Configs;
using System.Collections.Generic;
using CustomPlayerEffects;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Events.EventArgs;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core;
using PlayerStatsSystem;
using Utils.NonAllocLINQ;

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
                // do zombie stun
                if (_plugin.Config.ZombieLoadouts.Any(x => x.Roles.Any(x => x.Key == ev.Target.Role)))
                {
                    _plugin.Config.WeaponEffect.ApplyGunEffect(ref ev);
                    
                    ev.Attacker.ReceiveHitMarker();
                }
                // do player instakill
                if (_plugin.Config.ZombieLoadouts.Any(x => x.Roles.Any(x => x.Key == ev.Attacker.Role)) && 
                    _plugin.Config.MTFLoadouts.Any(x => x.Roles.Any(x => x.Key == ev.Target.Role)))
                {
                    ev.Amount = 0;
                    ev.Target.GiveLoadout(_plugin.Config.ZombieLoadouts);
                }

                /*if (ev.Attacker.IsHuman && ev.Target.IsSCP)
                {
                    ev.Target.EffectsManager.EnableEffect<Stained>(1);
                    ev.Target.EffectsManager.EnableEffect<Sinkhole>(1);
                }*/
            }
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            // if zombies are alive.
            if (Player.GetPlayers().Any(ply => _plugin.Config.ZombieLoadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ply.Role ))))
            {
                
                ev.Player.GiveLoadout(_plugin.Config.ZombieLoadouts);
                // Extensions.SetRole(ev.Player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = Player.GetPlayers().FirstOrDefault(x => x.IsSCP)!.Position;
                //RandomClass.GetSpawnPosition(_plugin.MapInfo.Map);

            }
            else
            {
                ev.Player.GiveLoadout(_plugin.Config.MTFLoadouts);
                // Extensions.SetRole(ev.Player, RoleTypeId.NtfSergeant, RoleSpawnFlags.AssignInventory);
                // ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.MapInfo.Map);
                ev.Player.Position = Player.GetPlayers().FirstOrDefault(x => !x.IsSCP)!.Position;
                // Extensions.SetPlayerAhp(ev.Player, 100, 100, 0);

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
