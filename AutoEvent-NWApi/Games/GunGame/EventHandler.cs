using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;

namespace AutoEvent.Games.GunGame
{
    public class EventHandler
    {
        Plugin _plugin;
        Dictionary<Player, Stats> _playerStats;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
            _playerStats = plugin.PlayerStats;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            if (!_playerStats.ContainsKey(ev.Player))
            {
                _playerStats.Add(ev.Player, new Stats { level = 1, kill = 0 });
            }

            Extensions.SetRole(ev.Player, GunGameRandom.GetRandomRole(), RoleSpawnFlags.None);
            ev.Player.Position = _plugin.SpawnPoints.RandomItem();
            GetWeaponForPlayer(ev.Player);
        } 
        
        public void OnPlayerDying(PlayerDyingArgs ev)
        {
            ev.IsAllowed = false;

            ev.Target.Health = 100;

            if (ev.Attacker != null)
            {
                _playerStats.TryGetValue(ev.Attacker, out Stats statsAttacker);

                statsAttacker.kill++;

                if (statsAttacker.kill >= 2)
                {
                    statsAttacker.kill = 0;
                    statsAttacker.level++;
                    GetWeaponForPlayer(ev.Attacker);
                }
            }

            if (ev.Target != null)
            {
                ev.Target.Position = _plugin.SpawnPoints.RandomItem();
                GetWeaponForPlayer(ev.Target);
            }
        }

        public void GetWeaponForPlayer(Player player)
        {
            player.IsGodModeEnabled = true;
            player.ClearInventory();
            var item = player.AddItem(GunGameGuns.GunByLevel[_playerStats[player].level]);

            Timing.CallDelayed(0.1f, () =>
            {
                player.CurrentItem = item;
            });

            Timing.CallDelayed(2f, () =>
            {
                player.IsGodModeEnabled = false;
            });
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
