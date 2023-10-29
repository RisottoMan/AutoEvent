// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         AbilityImplementations.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 4:39 PM
//    Created Date:     10/28/2023 4:39 PM
// -----------------------------------------

using System.Collections.Generic;
using AutoEvent.API;
using AutoEvent.API.Enums;
using CustomPlayerEffects;
using Footprinting;
using InventoryMenu.API;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using Mirror;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.Games.GhostBusters.Features;

public class AbilityImplementations
{
    private static GhostBusters.Plugin _plugin => (AutoEvent.ActiveEvent as GhostBusters.Plugin)!;

    public static bool CanPlayerUseAbility(Player ply, out GhostBusterClass? plyClass, bool applyUsage = true)
    {
        plyClass = null;
        if (!_plugin!.Classes.ContainsKey(ply))
        {
            ply.ReceiveHint("<color=red><b>Cannot Use Ability</b> \nYou are not spawned or an issue has occured.",7f);
            return false;
        }

        plyClass = _plugin.Classes[ply];
        if (!plyClass.IsAbilityReady(out string reason))
        {
            DebugLogger.LogDebug($"Player {ply.Nickname} cannot use ability. Reason: {reason}");
            ply.ReceiveHint($"<color=red><b>Cannot Use Ability</b>\n{reason}", 7f);
            return false;
        }

        if (!applyUsage)
            return true;
        
        plyClass.AbilityCooldown = plyClass.Ability!.Cooldown;
        plyClass.AbilityUses++;
        return true;
    }
    public static void UseInvisibilityAbility(Player ply, Ability ability)
    {
        if (!CanPlayerUseAbility(ply, out _))
            return;
        
        ply.GiveEffect(StatusEffect.Invisible, 1, 10f, false);
    }
    public static void UseTrapEffect(Player ply)
    {
        if (!CanPlayerUseAbility(ply, out _, false))
            return;
        
        
        var plyClass = _plugin.Classes[ply];
        plyClass.AbilityCooldown = plyClass.Ability!.Cooldown;
        plyClass.AbilityUses++;
    }
    public static void UseSpeedEffect(Player ply, Ability ability)
    {
        if (!CanPlayerUseAbility(ply, out var plyClass))
            return;
        
        ply.GiveEffect(StatusEffect.MovementBoost,20, plyClass!.Ability!.UseDuration, false);
        ply.ApplyFakeEffect<Scp207>(1);
        
        Timing.CallDelayed(plyClass.Ability!.UseDuration, () =>
        {
            ply.ApplyFakeEffect<Scp207>(0);
        });
    }

    public static void UseFlashEffect(Player ply, Ability ability)
    {
        if (!CanPlayerUseAbility(ply, out var plyClass))
            return;
        
        //todo make flashbangs invisible to teammates.
        _triggerFlashGrenade(ply);
    }

    private static void _triggerFlashGrenade(Player ply)
    {
        var identifier = new ItemIdentifier(ItemType.GrenadeFlash, ItemSerialGenerator.GenerateNext());
        var item = ReferenceHub.HostHub.inventory.CreateItemInstance(identifier, false) as ThrowableItem;

        TimeGrenade grenade = (TimeGrenade)Object.Instantiate(item.Projectile, ply.Position, Quaternion.identity);
        grenade._fuseTime = 0.1f;
        grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
        grenade.transform.localScale = new Vector3(1, 1, 1);

        NetworkServer.Spawn(grenade.gameObject);
        grenade.ServerActivate();
    } 
    
    public static void UseExplosionEffect(Player ply, Ability ability)
    {
        if (!CanPlayerUseAbility(ply, out var plyClass))
            return;
        
        _triggerGrenade(ply);    
    }
    private static void _triggerGrenade(Player ply)
    {
        var identifier = new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext());
        var item = ReferenceHub.HostHub.inventory.CreateItemInstance(identifier, false) as ThrowableItem;
        item.ExplodeOnCollision();

        TimeGrenade grenade = (TimeGrenade)Object.Instantiate(item.Projectile, ply.Position, Quaternion.identity);
        grenade._fuseTime = 3f;
        
        grenade.PreviousOwner = new Footprint(ply.ReferenceHub);
        grenade.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
        grenade.transform.localScale = new Vector3(1, 1, 1);

        NetworkServer.Spawn(grenade.gameObject);
        grenade.ServerActivate();
    } 

    public static void UseBallEffect(Player ply, Ability ability)
    {
        if (!CanPlayerUseAbility(ply, out var plyClass))
            return;
        _triggerBall(ply);
        
    }
    private static void _triggerBall(Player ply)
    {
        var itemBase = ply.AddItem(ItemType.SCP018);
        ply.HideMenu();
        ply.RefreshInventory();
        Timing.CallDelayed(0.1f, () =>
        {
            ply.CurrentItem = itemBase;
            var projectileSettings = ((ThrowableItem)itemBase); 
            double startVelocity = (double) projectileSettings.FullThrowSettings.StartVelocity;
            double upwardsFactor = (double) projectileSettings.FullThrowSettings.UpwardsFactor;
            Vector3 startTorque = projectileSettings.FullThrowSettings.StartTorque;
            Vector3 limitedVelocity = ThrowableNetworkHandler.GetLimitedVelocity(ply.Velocity);
            projectileSettings.ServerThrow((float)startVelocity * 3, (float)upwardsFactor, startTorque * 3, limitedVelocity);
            Timing.CallDelayed(1f, () => ply.ShowMenu(_plugin.GhostPowerupMenu));
            
        });
    } 
    
    public static void UseLockdownEffect(Player ply, Ability ability)
    {
        if (!CanPlayerUseAbility(ply, out var plyClass))
            return;

        _trigger2176(ply);
    }

    private static void _trigger2176(Player ply)
    {
        var itemBase = ply.AddItem(ItemType.SCP2176);
        ply.HideMenu();
        ply.RefreshInventory();
        Timing.CallDelayed(0.1f, () =>
        {
            ply.CurrentItem = itemBase;
            var projectileSettings = ((ThrowableItem)itemBase);
            double startVelocity = (double) projectileSettings.FullThrowSettings.StartVelocity;
            double upwardsFactor = (double) projectileSettings.FullThrowSettings.UpwardsFactor;
            Vector3 startTorque = projectileSettings.FullThrowSettings.StartTorque;
            Vector3 limitedVelocity = ThrowableNetworkHandler.GetLimitedVelocity(ply.Velocity);
            projectileSettings.ServerThrow((float)startVelocity * 3, (float)upwardsFactor, startTorque * 3, limitedVelocity);
            projectileSettings.gameObject.TryGetComponent<Scp2176Projectile>(out var comp);
            Timing.CallDelayed(1f, () => ply.ShowMenu(_plugin.GhostPowerupMenu));
            
        });
    }
    
}