using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using InventorySystem.Items;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Items;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void OnPickUpItem(PickUpItemArgs ev)
        {
            if (ev.Item.ItemTypeId != ItemType.SCP018)
                return;

            var trans = AutoEvent.Singleton.Translation.StrikeTranslation;

            if (_plugin.BombState == BombState.NoPlanted)
            {
                if (ev.Player.IsChaos)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Planted;
                    _plugin.RoundTime = new TimeSpan(0, 0, 35);

                    _plugin.BombObject.transform.position = ev.Pickup.Position;
                    _plugin.Buttons.ForEach(r => GameObject.Destroy(r));

                    Extensions.PlayAudio("BombPlanted.ogg", 5, false);
                    ev.Player.ReceiveHint(trans.StrikeYouPlanted, 3);
                }
            }
            else if (_plugin.BombState == BombState.Planted)
            {
                if (ev.Player.IsNTF && Vector3.Distance(ev.Player.Position, _plugin.BombObject.transform.position) < 3)
                {
                    _plugin.Winner = ev.Player;
                    _plugin.BombState = BombState.Defused;
                    //GameObject.Destroy(_plugin.BombObject);
                    ev.Player.ReceiveHint(trans.StrikeYouDefused, 3);
                }
            }

            ReturnButton(ev.Player, ev.Item, ev.Pickup.transform);
        }

        public void ReturnButton(Player player, ItemBase item, Transform transform)
        {
            // Create a new item.
            ItemPickup pickup = ItemPickup.Create(item.ItemTypeId, transform.position, transform.rotation);
            pickup.Transform.localScale = transform.localScale;

            Rigidbody rg = pickup.GameObject.GetComponent<Rigidbody>();
            rg.mass = 100;
            rg.drag = 0;
            rg.useGravity = false;
            rg.isKinematic = true;

            pickup.Spawn();

            // Removing an item from the player's inventory
            Timing.CallDelayed(0.2f, () =>
            {
                player.RemoveItems(ItemType.SCP018);
            });
        }

        [PluginEvent(ServerEventType.PlayerReloadWeapon)]
        public void OnReloading(PlayerReloadWeaponEvent ev)
        {
            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
            {
                ev.Player.SetAmmo(AmmoLimit.Key, AmmoLimit.Value);
            }
        }

        [PluginEvent(ServerEventType.PlayerSpawn)]
        public void OnSpawning(PlayerSpawnEvent ev)
        {
            foreach (KeyValuePair<ItemType, ushort> AmmoLimit in InventoryLimits.StandardAmmoLimits)
            {
                ev.Player.SetAmmo(AmmoLimit.Key, AmmoLimit.Value);
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
