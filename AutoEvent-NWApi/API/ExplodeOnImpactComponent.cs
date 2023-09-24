// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ExplodeOnImpactComponent.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/10/2023 6:59 PM
//    Created Date:     09/10/2023 6:59 PM
// -----------------------------------------

using System;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent;
public class GrenadeCollision : MonoBehaviour
{
    public TimeGrenade Grenade { get; private set; }

    public bool GiveOwnerNewGrenadeOnExplosion { get; set; } = false;

    public void Init(bool giveOwnerNewGrenadeOnExplosion = false)
    {
        GiveOwnerNewGrenadeOnExplosion = giveOwnerNewGrenadeOnExplosion;
    } 
    public void Awake()
    {
        Grenade = GetComponent<TimeGrenade>();
    }

    public void OnCollisionEnter()
    {
        Grenade.Network_syncTargetTime = NetworkTime.time + 0.05;
        if (GiveOwnerNewGrenadeOnExplosion)
        {
            if (Grenade.PreviousOwner.Hub is null)
                return;
            var player = Player.Get(Grenade.PreviousOwner.Hub);
            if (player is null)
                return;
            player.AddItem(Grenade.TryGetComponent<ExplosionGrenade>(out _) ? ItemType.GrenadeHE : ItemType.GrenadeFlash).ExplodeOnCollision();
        }
    }
}
