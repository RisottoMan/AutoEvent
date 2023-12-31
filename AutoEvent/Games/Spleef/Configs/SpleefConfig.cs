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
    
    [Description("The amount of health platforms have. Set to -1 to make them invincible.")]
    public float PlatformHealth { get; set; } = 1;

    [Description("A list of loadouts for spleef if a little count of players.")]
    public List<Loadout> PlayerLittleLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>()
            {
                ItemType.GunCom45,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
    [Description("A list of loadouts for spleef if a normal count of players.")]
    public List<Loadout> PlayerNormalLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>()
            {
                ItemType.GunCOM18,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
    [Description("A list of loadouts for spleef if a big count of players.")]
    public List<Loadout> PlayerBigLoadouts { get; set; } = new List<Loadout>()
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