using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using MER.Lite;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Games.CounterStrike
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
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

        public void OnPickUpItem(PickUpItemArgs ev)
        {
            if (ev.Item.ItemTypeId != ItemType.SCP018) return;

            if (_plugin.BombState == BombState.NoPlanted)
            {
                if (ev.Player.IsChaos)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Planted;
                    _plugin.RoundTime = new TimeSpan(0, 0, 35);
                    _plugin.BombPoints.ForEach(r => GameObject.Destroy(r));
                    _plugin.BombSchematic = ObjectSpawner.SpawnSchematic("bomb", ev.Pickup.Position, Quaternion.identity, Vector3.one);

                    Extensions.PlayAudio("BombPlanted.ogg", 5, false, "BombPlanted");
                    ev.Player.ReceiveHint("Вы устанавили бомбу", 3);
                }
                else
                {
                    ev.Player.ReceiveHint("Бомба не установлена", 3);
                }
            }
            else if (_plugin.BombState == BombState.Planted)
            {
                if (ev.Player.IsNTF && Vector3.Distance(ev.Player.Position, _plugin.BombSchematic.Position) < 3)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Defused;
                    _plugin.BombSchematic.Destroy();
                    ev.Player.ReceiveHint("Вы задефьюзили бомбу", 3);
                }
                else
                {
                    ev.Player.ReceiveHint("Бомба уже установлена", 3);
                }
            }

            ev.IsAllowed = false;
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
