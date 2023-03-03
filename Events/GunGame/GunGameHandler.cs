using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static AutoEvent.Events.GunGameEvent;
using Object = UnityEngine.Object;

namespace AutoEvent.Events
{
    internal class GunGameHandler
    {
        public static void OnJoin(VerifiedEventArgs ev)
        {
            PlayerStats.Add(ev.Player, new Stats
            {
                kill = 0,
                level = 1
            });

            ev.Player.Role.Set(GunGameRandom.GetRandomRole());
            ev.Player.ClearInventory();
            ev.Player.CurrentItem = Item.Create(GunGameGuns.GunForLevel[PlayerStats[ev.Player].level], ev.Player);
            ev.Player.Position = GameMap.Position + GunGameRandom.GetRandomPosition();
        }
        public static void OnDied(DiedEventArgs ev)
        {
            // Attacker shit
            if (ev.Attacker != null)
            {
                PlayerStats.TryGetValue(ev.Attacker, out Stats statsAttacker);
                statsAttacker.kill++;
                if (statsAttacker.kill >= 2)
                {
                    statsAttacker.level++;
                    statsAttacker.kill = 0;

                    ev.Attacker.ClearInventory();
                    ev.Attacker.CurrentItem = Item.Create(GunGameGuns.GunForLevel[PlayerStats[ev.Attacker].level], ev.Attacker);
                }
            }
            // Target shit
            if (ev.Player != null)
            {
                ev.Player.Role.Set(GunGameRandom.GetRandomRole());
                ev.Player.IsSpawnProtected = false;
                ev.Player.ClearInventory();
                ev.Player.CurrentItem = Item.Create(GunGameGuns.GunForLevel[PlayerStats[ev.Player].level], ev.Player);
                ev.Player.Position = GameMap.Position + GunGameRandom.GetRandomPosition();
            }
        }
        public static void OnSpawned(SpawnedEventArgs ev)
        {
            ev.Player.AddAmmo(AmmoType.Nato9, 100);
            ev.Player.AddAmmo(AmmoType.Ammo44Cal, 100);
            ev.Player.AddAmmo(AmmoType.Nato556, 100);
            ev.Player.AddAmmo(AmmoType.Nato762, 100);
            ev.Player.AddAmmo(AmmoType.Ammo12Gauge, 100);
        }
        public static void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public static void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public static void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public static void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public static void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;

    }
}
