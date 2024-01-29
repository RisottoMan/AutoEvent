using AutoEvent.Events.EventArgs;
using CustomPlayerEffects;
using InventorySystem.Configs;
using MEC;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Deathmatch
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
            int mtfCount = Player.GetPlayers().Count(r => r.Team == Team.FoundationForces);
            int chaosCount = Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency);
            if (mtfCount > chaosCount)
            {
                ev.Player.GiveLoadout(_plugin.Config.ChaosLoadouts);
            }
            else
            {
                ev.Player.GiveLoadout(_plugin.Config.NTFLoadouts);
            }
            
            ev.Player.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);

            Timing.CallDelayed(.1f, () =>
            {
                if (ev.Player.Items.First() != null)
                {
                    ev.Player.CurrentItem = ev.Player.Items.First();
                }
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

        public void OnPlayerDying(PlayerDyingArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.Target.Team == Team.FoundationForces)
            {
                _plugin.ChaosKills++;
            }
            else if (ev.Target.Team == Team.ChaosInsurgency)
            {
                _plugin.MtfKills++;
            }

            ev.Target.EffectsManager.EnableEffect<Flashed>(0.1f);
            ev.Target.Position = RandomClass.GetRandomPosition(_plugin.MapInfo.Map);
            ev.Target.IsGodModeEnabled = true;
            ev.Target.Health = 100;

            List<ItemType> itemsToDrop = new List<ItemType>();
            foreach (var itemBase in ev.Target.Items)
            {
                if (itemBase.ItemTypeId.IsWeapon())
                {
                    itemsToDrop.Add(itemBase.ItemTypeId);
                }
            }

            foreach (var itemType in itemsToDrop)
            {
                ev.Target.RemoveItems(itemType);
            }
            var item = ev.Target.AddItem(_plugin.Config.AvailableWeapons.RandomItem());
            Timing.CallDelayed(.1f, () =>
            {
                ev.Target.IsGodModeEnabled = false;
                if (item != null)
                {
                    ev.Target.CurrentItem = item;
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
