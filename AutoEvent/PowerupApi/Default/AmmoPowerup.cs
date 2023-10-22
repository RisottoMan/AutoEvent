// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         AmmoPowerup.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 12:49 PM
//    Created Date:     10/17/2023 12:49 PM
// -----------------------------------------

using System.Collections.Generic;
using CustomPlayerEffects;
using Exiled.API.Features.Items;
using PluginAPI.Core;
using UnityEngine;

namespace Powerups.Default;

public class AmmoPowerup : Powerup
{
    public AmmoPowerup()
    {
        // one box of ammo for each type of ammo.
        AmmoToGive = new Dictionary<ItemType, ushort>()
        {
            { ItemType.Ammo9x19, 30 },
            { ItemType.Ammo762x39, 60 },
            { ItemType.Ammo556x45, 80 },
            { ItemType.Ammo44cal, 12 },
            { ItemType.Ammo12gauge, 28 },
        };
    }
    public AmmoPowerup(Dictionary<ItemType, ushort> ammoToGive)
    {
        AmmoToGive = ammoToGive;
    }
    public override string Name { get; protected set; } = "Ammo";
    public override string Description { get; protected set; } = "Gives a player ammo.";
    protected override string SchematicName { get; set; } = "5.56";
    protected override Vector3 SchematicScale { get; set; } = new Vector3(20, 20, 20);
    protected override Vector3 ColliderScale { get; set; } = new Vector3(0.1f, 0.1f, 0.1f);
    public override float PowerupDuration { get; protected set; } = 0;
    public Dictionary<ItemType, ushort> AmmoToGive { get; set; }

    protected override void OnApplyPowerup(Player ply)
    {
        foreach (var kvp in this.AmmoToGive)
        {
            ply.AddAmmo(kvp.Key, (ushort)kvp.Value);
        }
    }
}