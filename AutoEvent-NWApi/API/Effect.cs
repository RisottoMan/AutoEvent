// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Effect.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/17/2023 2:28 PM
//    Created Date:     09/17/2023 2:28 PM
// -----------------------------------------

using System;
using System.ComponentModel;

namespace AutoEvent.API.Enums;

[Description("An effect that can be applied to a player.")]
public class Effect
{
    public Effect() {}

    public Effect(StatusEffect effectType, byte intensity = 1, float duration = 0, bool addDuration = false)
    {
        EffectType = effectType;
        Intensity = intensity;
        Duration = duration;
        AddDuration = addDuration;
    }
    [Description("The type of effect the player can have.")]
    public StatusEffect EffectType { get; set; }

    [Description($"The intensity of the effect.")]
    public byte Intensity { get; set; } = 1;

    [Description($"The duration of the effect. 0 is infinite.")]
    public float Duration { get; set; } = 0;

    [Description($"Should the effect add duration if the effect already is applied.")]
    public bool AddDuration { get; set; } = false;

    public override string ToString()
    {
        // Scp207 x4 [Infinite]
        // Scp207 x4 [10:00]
        // Scp207 x4 [00:05]
        return $"{EffectType.ToString()} x{Intensity} [{(Duration == 00 ? "Infinite" : $"{TimeSpan.FromSeconds(Duration).Minutes:00}:{TimeSpan.FromSeconds(Duration).Seconds:00}")}] ";
    }
}