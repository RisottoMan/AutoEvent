using AutoEvent.API.Enums;
using AutoEvent.Events.EventArgs;
using CustomPlayerEffects;
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
            if (!_plugin.TotalKills.ContainsKey(ev.Player))
            {
                _plugin.TotalKills.Add(ev.Player, 0);
            }

            SpawnPlayerAfterDeath(ev.Player);
        }


        [PluginEvent(ServerEventType.PlayerLeft)]
        public void OnLeft(PlayerLeftEvent ev)
        {
            if (_plugin.TotalKills.ContainsKey(ev.Player))
            {
                _plugin.TotalKills.Remove(ev.Player);
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

        public void OnPlayerDying(PlayerDyingArgs ev)
        {
            ev.IsAllowed = false;
            _plugin.TotalKills[ev.Attacker]++;

            SpawnPlayerAfterDeath(ev.Target);
        }

        public void SpawnPlayerAfterDeath(Player player)
        {
            player.EffectsManager.EnableEffect<Flashed>(0.1f);
            player.IsGodModeEnabled = true;
            player.Health = 100;
            player.ClearInventory();

            if (!player.IsAlive)
            {
                player.GiveLoadout(_plugin.Config.NTFLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
            }

            player.Position = _plugin.Points.RandomItem().transform.position;

            var item = player.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
            Timing.CallDelayed(.1f, () =>
            {
                player.IsGodModeEnabled = false;
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
