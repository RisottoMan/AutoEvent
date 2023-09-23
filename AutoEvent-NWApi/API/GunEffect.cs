// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GunEffect.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/22/2023 6:46 PM
//    Created Date:     09/22/2023 6:46 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API.Enums;

namespace AutoEvent.API;

public class GunEffect
{
    public GunEffect() { }

    public GunEffect(float damage, List<Effect> effects)
    {
        Damage = damage;
        Effects = effects;
    }
    [Description("The damage that guns do.")]
    public float Damage { get; set; } = 3f;

    [Description("The effects that guns give.")]
    public List<Effect> Effects { get; set; } = new List<Effect>();
}