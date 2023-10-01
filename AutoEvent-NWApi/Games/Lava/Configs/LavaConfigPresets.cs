// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         LavaConfigPreset.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/24/2023 1:57 PM
//    Created Date:     09/24/2023 1:57 PM
// -----------------------------------------

using System.Collections.Generic;
using AutoEvent.API;

namespace AutoEvent.Games.Infection;

public static class LavaConfigPreset
{
    public static LavaConfig Original = new LavaConfig()
    {
        ItemsAndWeaponsToSpawn = new Dictionary<ItemType, float>(),
        GunEffects = new List<WeaponEffect>(),
    };
}