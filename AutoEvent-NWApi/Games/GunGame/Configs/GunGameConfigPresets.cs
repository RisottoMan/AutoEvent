// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GunGameConfigPresets.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/26/2023 9:02 PM
//    Created Date:     09/26/2023 9:02 PM
// -----------------------------------------

using System.Collections.Generic;

namespace AutoEvent.Games.Infection;

public static class GunGameConfigPresets
{
    public static GunGameConfig EasyGunsFirst => new GunGameConfig()
    {
        Guns = new List<GunRole>()
        {
            new GunRole(ItemType.GunFRMG0, 0),
            new GunRole(ItemType.GunE11SR, 1),
            new GunRole(ItemType.GunA7, 2),
            new GunRole(ItemType.GunLogicer, 3),
            new GunRole(ItemType.GunAK, 4),
            new GunRole(ItemType.GunFSP9, 5),
            new GunRole(ItemType.GunAK, 6),
            new GunRole(ItemType.GunCrossvec, 7),
            new GunRole(ItemType.GunE11SR, 8),
            new GunRole(ItemType.GunRevolver, 9),
            new GunRole(ItemType.GunShotgun, 10),
            new GunRole(ItemType.GunCOM18, 11),
            new GunRole(ItemType.GunRevolver, 12),
            new GunRole(ItemType.GunCOM15, 13),
            new GunRole(ItemType.ParticleDisruptor, 14),
            new GunRole(ItemType.GunCom45, 15),
            new GunRole(ItemType.Jailbird, 16),
            new GunRole(ItemType.None, 17)
        }
    };
}