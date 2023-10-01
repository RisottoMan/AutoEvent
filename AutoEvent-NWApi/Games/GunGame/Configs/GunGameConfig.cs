// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GunGameConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 9:06 PM
//    Created Date:     09/18/2023 9:06 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using CommandSystem.Commands.RemoteAdmin;
using PlayerRoles;

namespace AutoEvent.Games.Infection;

public class GunGameConfig : EventConfig
{
    [Description("A list of guns a player can get.")]
    public List<GunRole> Guns { get; set; } = new List<GunRole>()
    {
        new GunRole(ItemType.GunCOM15, 0),
        new GunRole(ItemType.GunCOM18, 1),
        new GunRole(ItemType.GunRevolver, 2),
        new GunRole(ItemType.GunCom45, 3),
        new GunRole(ItemType.GunFSP9, 4),
        new GunRole(ItemType.GunCrossvec, 5),
        new GunRole(ItemType.GunAK, 6),
        new GunRole(ItemType.GunShotgun, 7),
        new GunRole(ItemType.GunE11SR, 8),
        new GunRole(ItemType.GunRevolver, 9),
        new GunRole(ItemType.ParticleDisruptor, 10),
        new GunRole(ItemType.GunA7, 11),
        new GunRole(ItemType.GunAK, 12),
        new GunRole(ItemType.GunE11SR, 13),
        new GunRole(ItemType.GunLogicer, 14),
        new GunRole(ItemType.GunFRMG0, 15),
        new GunRole(ItemType.Jailbird, 16),
        new GunRole(ItemType.None, 17),
    };

    [Description("The loadouts a player can get.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Health = 100,
            InfiniteAmmo = AmmoMode.InfiniteAmmo,
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 20 },
                { RoleTypeId.Scientist, 20 },
                { RoleTypeId.NtfSergeant, 20 },
                { RoleTypeId.ChaosRifleman, 20 },
                { RoleTypeId.FacilityGuard, 20 },
            }
        }
    };

}

public class GunRole
{
    public GunRole()
    {
        
    }

    public GunRole(ItemType item, int killsRequired)
    {
        Item = item;
        KillsRequired = killsRequired;
    }
    [Description("The weapon that the player will recieve once they get to this role.")]
    public ItemType Item { get; set; } = ItemType.GunCOM15;
    
    [Description("Total kills needed to get this gun. [Default: 1]")]
    public int KillsRequired { get; set; } = 1;

    
}