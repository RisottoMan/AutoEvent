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
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent;

public class ExplodeOnImpactComponent : MonoBehaviour
{
    private bool initialized;
    public GameObject Owner { get; private set; }

    public TimeGrenade Grenade { get; private set; }

    public void Init(GameObject owner, ThrownProjectile grenade)
    {
        Owner = owner;
        Grenade = (TimeGrenade)grenade;
        initialized = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (!initialized)
                return;
            if (Owner == null)
                Log.Error($"Owner is null!");
            if (Grenade == null)
                Log.Error($"Grenade is null!");
            if (collision is null)
                Log.Error($"wat");
            if (collision.gameObject == null)
                Log.Error($"clueless");
            if (collision.gameObject == Owner || collision.gameObject.TryGetComponent<EffectGrenade>(out _))
                return;

            Grenade.TargetTime = 0.1;
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(OnCollisionEnter)} error:\n{e}");
            Destroy(this);
        }
    }
}