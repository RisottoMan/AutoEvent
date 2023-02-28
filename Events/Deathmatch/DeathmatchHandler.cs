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
            ev.Player.Role.Set(RoleTypeId.NtfCaptain);
            ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
            ev.Player.Position = DeathmatchEvent.GameMap.Position + DeathmatchRandom.GetRandomPosition();
        }
        public static void OnDied(DiedEventArgs ev)
        {
            if (ev.TargetOldRole.GetTeam() == Team.FoundationForces)
            {
                DeathmatchEvent.ChaosKills++;
            }
            else if (ev.TargetOldRole.GetTeam() == Team.ChaosInsurgency)
            {
                DeathmatchEvent.MtfKills++;
            }
            Timing.CallDelayed(3f, () =>
            {
                if (ev.TargetOldRole.GetTeam() == Team.FoundationForces)
                {
                    ev.Player.Role.Set(DeathmatchClass.GetRandomClass(Team.FoundationForces));
                }
                else if (ev.TargetOldRole.GetTeam() == Team.ChaosInsurgency)
                {
                    ev.Player.Role.Set(DeathmatchClass.GetRandomClass(Team.ChaosInsurgency));
                }
                // Spawn Protect
                ev.Player.IsSpawnProtected = false;
                // Get items
                ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
                // Get effects
                ev.Player.EnableEffect<CustomPlayerEffects.Scp1853>(15);
                ev.Player.EnableEffect(EffectType.MovementBoost, 15);
                ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 25);
                // Get pos
                ev.Player.Position = DeathmatchEvent.GameMap.Position + DeathmatchRandom.GetRandomPosition();
            });
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
    }
}
