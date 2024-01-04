using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using InventorySystem.Items.Pickups;
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

        public void OnPickupItem(PickUpItemArgs ev)
        {
            if (ev.Item.ItemTypeId == ItemType.Medkit)
            {
                DebugLogger.LogDebug("Pick up item = false;", LogLevel.Error);
                ev.IsAllowed = false;
            }
        }

        public void OnSearchPickUpItem(SearchPickUpItemArgs ev)
        {
            if (ev.Item.ItemTypeId == ItemType.MicroHID)
            {
                DebugLogger.LogDebug("Search pick up item = false;", LogLevel.Error);
                ev.IsAllowed = false;
            }
        }

        public void OnPlayerNoclip(PlayerNoclipArgs ev)
        {
            StrikeTranslation translation = AutoEvent.Singleton.Translation.StrikeTranslation;

            if (_plugin.BombState == BombState.NoPlanted && ev.Player.IsChaos)
            {
                foreach (var point in _plugin.BombPoints)
                {
                    if (Vector3.Distance(point.transform.position, ev.Player.Position) < 2)
                    {
                        // need add timing
                        _plugin.Winner = ev.Player;
                        _plugin.BombState = BombState.Planted;
                        _plugin.RoundTime = new TimeSpan(0, 0, 35);
                        _plugin.BombSchematic = ObjectSpawner.SpawnSchematic(
                            "bomb",
                            point.transform.position,
                            Quaternion.Euler(ev.Player.Rotation),
                            Vector3.one);

                        Extensions.PlayAudio("BombPlanted.ogg", 5, false);
                        ev.Player.ReceiveHint(translation.StrikeYouPlanted, 3);
                    }
                }
            }
            else if (_plugin.BombState == BombState.Planted && ev.Player.IsNTF)
            {
                if (Vector3.Distance(ev.Player.Position, _plugin.BombSchematic.Position) < 3)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Defused;
                    _plugin.BombSchematic.Destroy();
                    ev.Player.ReceiveHint(translation.StrikeYouDefused, 3);
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerUsingRadio)]
        public void OnUsingRadio(PlayerUsingRadioEvent ev)
        {
            /*
            if (ev.Player.Team == Team.FoundationForces)
            {
                ev.Radio._rangeId = 3;
                ev.Radio.
            }
            else
            {
                ev.Radio._rangeId = 2;
            }

            ev.Drain = 0;
            */
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
