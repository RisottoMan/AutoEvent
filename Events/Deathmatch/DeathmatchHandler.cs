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
using Object = UnityEngine.Object;

namespace AutoEvent.Events
{
    internal class DeathmatchHandler
    {
        public static void OnJoin(VerifiedEventArgs ev)
        {
            // Give role
            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) > Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency))
            {
                ev.Player.Role.Set(RoleTypeId.ChaosRifleman);
            }
            else
            {
                ev.Player.Role.Set(RoleTypeId.NtfSergeant);
            }
            ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
            // Effects
            ev.Player.EnableEffect<CustomPlayerEffects.Scp1853>(300);
            ev.Player.EnableEffect(EffectType.MovementBoost, 300);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 25);
            ev.Player.EnableEffect<CustomPlayerEffects.SpawnProtected>(5);
            // Position
            ev.Player.Position = DeathmatchEvent.GameMap.Position + DeathmatchRandom.GetRandomPosition();
        }
        public static void OnDying(DyingEventArgs ev)
        {
            // We don't kill the player, but move them around the arena.
            ev.IsAllowed = false;
            // Get exp
            if (ev.Player.Role.Team == Team.FoundationForces)
            {
                DeathmatchEvent.ChaosKills++;
            }
            else if (ev.Player.Role.Team == Team.ChaosInsurgency)
            {
                DeathmatchEvent.MtfKills++;
            }
            // Respawn player
            ev.Player.EnableEffect(EffectType.Flashed, 0.1f);
            ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
            ev.Player.Position = DeathmatchEvent.GameMap.Position + DeathmatchRandom.GetRandomPosition();
            ev.Player.EnableEffect<CustomPlayerEffects.SpawnProtected>(1);
            ev.Player.Health = 100;
        }
        public static void OnShooting(ShootingEventArgs ev)
        {
            switch (ev.Player.Inventory.CurItem.TypeId)
            {
                case (ItemType.GunCOM15):
                case (ItemType.GunCOM18):
                case (ItemType.GunCrossvec):
                case (ItemType.GunFSP9): { ev.Player.AddAmmo(AmmoType.Nato9, 1); } break;
                case (ItemType.GunRevolver): { ev.Player.AddAmmo(AmmoType.Ammo44Cal, 1); } break;
                case (ItemType.GunE11SR): { ev.Player.AddAmmo(AmmoType.Nato556, 1); } break;
                case (ItemType.GunAK):
                case (ItemType.GunLogicer): { ev.Player.AddAmmo(AmmoType.Nato762, 1); } break;
                case (ItemType.GunShotgun): { ev.Player.AddAmmo(AmmoType.Ammo12Gauge, 1); } break;
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
        public static void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public static void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public static void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public static void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public static void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public static void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
