// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SpeedPowerup.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 12:55 PM
//    Created Date:     10/17/2023 12:55 PM
// -----------------------------------------

using CustomPlayerEffects;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using PluginAPI.Core;
using Powerups.Extensions;
using UnityEngine;

namespace Powerups.Default;

public sealed class SpeedPowerup : Powerup
{
    public override string Name { get; protected set; } = "Speed";
    public override string Description { get; protected set; } = "Gives a player a temporary speed boost";
    protected override string SchematicName { get; set; } = "Bepis";
    protected override Vector3 SchematicScale { get; set; } = new Vector3(3, 3, 3);
    protected override Vector3 ColliderScale { get; set; } = new Vector3(0.3f, 0.3f, 0.3f);

    public SpeedPowerup()
    {
        this.PowerupDuration = 15f;
        this.SpeedIntensity = 30;
    }
    public SpeedPowerup(float duration = 15f, byte speed = 30)
    {
        this.PowerupDuration = duration;
        this.SpeedIntensity = speed;
    }
    public void SetPowerupParameters(float duration = 15f, byte speed = 30)
    {
        this.PowerupDuration = duration;
        this.SpeedIntensity = speed;
    }

    public override float PowerupDuration { get; protected set; } = 15;
    public byte SpeedIntensity { get; private set; }
    protected override void OnApplyPowerup(Player ply)
    {
        ply.EffectsManager.EnableEffect<MovementBoost>().Intensity = SpeedIntensity;
        ply.ApplyFakeEffect<Scp207>(1);
    }

    protected override void OnRemovePowerup(Player ply)
    {
        ply.EffectsManager.DisableEffect<MovementBoost>();
        ply.ApplyFakeEffect<Scp207>(0);
    }
}