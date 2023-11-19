using AutoEvent.API.Enums;
using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using MEC;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.AllDeathmatch
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            SpawnPlayerAfterDeath(ev.Player);
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

        public void OnPlayerDying(PlayerDyingArgs ev)
        {
            Timing.CallDelayed(5f, () =>
            {
                SpawnPlayerAfterDeath(ev.Target);
            });
        }

        public void SpawnPlayerAfterDeath(Player player)
        {
            player.GiveLoadout(_plugin.Config.NTFLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
            player.Position = _plugin.Spawnpoints.RandomItem().transform.position;

            var item = player.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
            Timing.CallDelayed(.1f, () =>
            {
                if (item != null)
                {
                    player.CurrentItem = item;
                }
            });
        }

        public void OnHandCuff(HandCuffArgs ev) => ev.IsAllowed = false;
        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
