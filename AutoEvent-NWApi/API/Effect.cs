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
    [Description("The type of effect the player can have.")]
    public StatusEffect Type { get; set; }
    
    [Description($"The intensity of the effect.")]
    public byte Intensity { get; set; }

    [Description($"The duration of the effect. 0 is infinite.")]
    public float Duration { get; set; }
    
    [Description($"Should the effect add duration if the effect already is applied.")]
    public bool AddDuration { get; set; }

    public override string ToString()
    {
        // Scp207 x4 [Infinite]
        // Scp207 x4 [10:00]
        // Scp207 x4 [00:05]
        return $"{Type.ToString()} x{Intensity} [{(Duration == 00 ? "Infinite" : $"{TimeSpan.FromSeconds(Duration).Minutes:00}:{TimeSpan.FromSeconds(Duration).Seconds:00}")}] ";
    }
}