// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EventHandler.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 6:20 PM
//    Created Date:     10/17/2023 6:20 PM
// -----------------------------------------

using AutoEvent.API.Components;
using AutoEvent.Events.EventArgs;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace AutoEvent.Games.Spleef;

public class EventHandler
{
    [PluginEvent(ServerEventType.PlayerShotWeapon)]
    public void OnShooting(PlayerShotWeaponEvent ev)
    {
        if (Physics.Raycast(ev.Player.Position, ev.Player.ReferenceHub.transform.forward, out RaycastHit hit, 20))
        {
            var destructible = hit.collider.GetComponent<DestructiblePrimitiveComponent>();
            if (destructible is null)
            {
                DebugLogger.LogDebug("Destructible is null.");
                return;
            }
            var damageHandler = new FirearmDamageHandler(ev.Firearm, 1000, false);
            if (damageHandler is null)
            {
                DebugLogger.LogDebug("Damagehandler is null");
            
            }
            bool result = destructible.Damage(1000, damageHandler, hit.point);
            DebugLogger.LogDebug($"Result: {result}");
            
        }
    }
    

    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}