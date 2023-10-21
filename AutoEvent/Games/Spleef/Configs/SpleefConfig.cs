// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SpleefConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 6:20 PM
//    Created Date:     10/17/2023 6:20 PM
// -----------------------------------------

using System.Collections.Generic;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Spleef.Configs;

public class SpleefConfig : EventConfig
{
    public int RoundDurationInSeconds { get; set; } = 300;
    public int PlatformAxisCount { get; set; } = 20;
    public int LayerCount { get; set; } = 3;

    public List<Loadout> PlayerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>()
            {
                ItemType.GunCrossvec,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}