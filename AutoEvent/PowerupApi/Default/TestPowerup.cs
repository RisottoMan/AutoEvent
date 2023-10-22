// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         TestPowerup.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 1:32 AM
//    Created Date:     10/17/2023 1:32 AM
// -----------------------------------------
using CustomPlayerEffects;
using PluginAPI.Core;
using UnityEngine;

namespace Powerups.Default;

public class TestPowerup : Powerup
{
    public override string Name { get; protected set; } = "Test";
    public override string Description { get; protected set; } = "A dummy powerup for testing purposes.";
    protected override string SchematicName { get; set; } = "Pills";
    public override float PowerupDuration { get; protected set; } = 60;
    protected override void OnApplyPowerup(Player ply)
    {
        ply.EffectsManager.DisableEffect<RainbowTaste>();
        ply.EffectsManager.DisableEffect<MovementBoost>();
    }

    protected override void OnRemovePowerup(Player ply)
    {
        ply.EffectsManager.EnableEffect<RainbowTaste>(0f, false);
        ply.EffectsManager.EnableEffect<MovementBoost>(0f, false).Intensity = 30;
    }
}
