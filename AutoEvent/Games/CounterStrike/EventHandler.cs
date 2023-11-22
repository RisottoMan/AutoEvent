using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;

namespace AutoEvent.Games.CounterStrike
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        [PluginEvent(ServerEventType.PlayerSpawn)]
        public void OnSpawning(PlayerSpawnEvent ev)
        {
            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
            {
                ev.Player.SetAmmo(AmmoLimit.Key, AmmoLimit.Value);
            }
        }

        public void OnUsingItem(UsingItemEvent ev)
        {
            if (ev.Item.Type != ItemType.SCP018) return;
            if (_plugin.BombState == BombState.NoPlanted)
            {
                if (ev.Player.isChaos)
                {
                    // Вы устанавливаете бомбу
                }
                else
                {
                    // Бомба не установлена
                }
            }
            
            if (_plugin.BombState == BombState.Planted)
            {
                if (ev.Player.isNtf)
                {
                    // Вы дефьюзете бомбу
                }
                else
                {
                    // Бомба уже установлена
                }
            }    

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            Extensions.SetRole(ev.Player, RoleTypeId.Spectator, RoleSpawnFlags.None);
        }
        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
