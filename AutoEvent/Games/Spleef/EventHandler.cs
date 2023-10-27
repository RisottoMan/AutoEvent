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

using System;
using AutoEvent.API.Components;
using AutoEvent.Events.EventArgs;
using AutoEvent.Games.Spleef.Features;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace AutoEvent.Games.Spleef;

public class EventHandler
{
    private Plugin _plugin { get; set; }
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }
    public void OnShot(ShotEventArgs ev)
    {
        if (_plugin.Config.PlatformHealth < 0)
        {
            return;
        }

        if (ev.Player.CurrentItem is not Firearm firearm)
        {
            return;
        }
        if (ev.Damage <= 0)
        {
            ev.Damage = BodyArmorUtils.ProcessDamage(0, firearm.BaseStats.DamageAtDistance(firearm, ev.Distance), Mathf.RoundToInt(firearm.ArmorPenetration * 100f));
        }
        ev.RaycastHit.collider.transform.GetComponentsInParent<FallPlatformComponent>().ForEach(x =>
        {
            var damageHandler = new FirearmDamageHandler(ev.Player.CurrentItem as Firearm, ev.Damage, false);
            bool result = x.Damage(ev.Damage, damageHandler, ev.RaycastHit.point);
        });
    }


    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}