// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         JailConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 1:45 PM
//    Created Date:     09/19/2023 1:45 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Infection;

public class JailConfig : EventConfig
{
    [Description($"A list of loadouts for the jailors.")]
    public List<Loadout> JailorLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfCaptain, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR,
                ItemType.GunCOM18
            }
        },
    };

    [Description("A list of loadouts for the prisoners.")]
    public List<Loadout> PrisonerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 }
            }
        }
    };
    
    [Description("What loadouts each locker can give.")]
    public List<Loadout> WeaponLockerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR,
                ItemType.GunCOM15
            },
        },
        new Loadout()
        {
            Items = new List<ItemType>()
            {
                ItemType.GunCrossvec,
                ItemType.GunRevolver
            }
        },
    };

    public List<Loadout> MedicalLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Health = 100
        }
    };

    public List<Loadout> AdrenalineLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            ArtificialHealth = 100
        }
    };
}