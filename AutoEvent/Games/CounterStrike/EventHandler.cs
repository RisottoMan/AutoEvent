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

        public void OnPickUpItem(PickUpItemEvent ev)
        {
            if (ev.Item.Type != ItemType.SCP018) return;

            ev.IsAllowed = false;
            if (_plugin.BombState == BombState.NoPlanted)
            {
                if (ev.Player.isChaos)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Planted;
                    _plugin.BombSchematic = MER.Api.SpawnSchematic("Bomb", ev.Item.Position, Quartenion.Identity, Vector3.one);

                    _plugin.EventTime = new TimeSpan(0, 0, 35);
                    Extensions.PlayAudio("BombPlanted", 5, false);
                    ev.Player.ReceiveHint("<color=red>Вы устанавливаете бомбу</color>", 3);
                }
                else
                {
                    ev.Player.ReceiveHint("Бомба не установлена", 3);
                }
            }
            else if (_plugin.BombState == BombState.Planted)
            {
                if (ev.Player.isNtf)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Defused;
                    _plugin.BombSchematic.Destroy();
                    ev.Player.ReceiveHint("Вы дефьюзете бомбу", 3);
                }
                else
                {
                    ev.Player.ReceiveHint("Бомба уже установлена", 3);
                }
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
