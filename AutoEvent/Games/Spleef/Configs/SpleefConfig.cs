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
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Spleef.Configs;

public class SpleefConfig : EventConfig
{
    [Description("How long the round should last.")]
    public int RoundDurationInSeconds { get; set; } = 300;

    [Description("Set to -1 to disable. Causes platforms to regenerate after a certain number of seconds.")]
    public int RegeneratePlatformsAfterXSeconds { get; set; } = -1;

    [Description("How many platforms on the x and y axis. Total Platforms = PlatformAxisCount * PlatformAxisCount * LayerCount. Size is automatically determined.")]
    public int PlatformAxisCount { get; set; } = 20;
    [Description("How many \"levels\" of height should be in the map. ")]
    public int LayerCount { get; set; } = 3;

    [Description("How long before platforms will fall after being stepped on. Set to -1 to disable this.")]
    public float PlatformFallDelay { get; set; } = -1;
    
    [Description("The amount of health platforms have. Set to -1 to make them invincible.")]
    public float PlatformHealth { get; set; } = 1;

    [Description("A list of loadouts for spleef.")]
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
                ItemType.GunCOM15,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}