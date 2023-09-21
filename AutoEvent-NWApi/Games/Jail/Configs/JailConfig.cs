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
using YamlDotNet.Serialization;

namespace AutoEvent.Games.Infection;

public class JailConfig : EventConfig
{
    [Description($"A list of loadouts for the jailors.")]
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
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
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
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
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
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

    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
    public List<Loadout> MedicalLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Health = 100
        }
    };
    
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
    public List<Loadout> AdrenalineLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            ArtificialHealth = 100
        }
    };
}