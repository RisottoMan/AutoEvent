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

    [Description("How long before prisoners are released automatically. -1 to disable.")]
    public float AutoReleaseDelayInSeconds { get; set; } = 20f;

    [Description("How many lives each prisoner gets.")]
    public int PrisonerLives { get; set; } = 3;

    [Description("How long the \"Jail Lockdown Button\" will lock the prisoner doors. O5 and containment engineer will double the duration. Bypass mode will permanently lockdown.")]
    public float LockdownDurationInSeconds { get; set; } = 15f;
    
    [Description("How long the cooldown is for the jail lockdown. Bypass mode will skip this cooldown.")]
    public float LockdownCooldownDurationInSeconds { get; set; } = 15f;
    
    
    [Description("How many players will spawn as the jailors.")]
    public RoleCount JailorRoleCount { get; set; } = new RoleCount(1, 4, 15f);
    
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
            ArtificialHealth = new ArtificialHealth(50f, 50f, -3f, 70, false, 10f, true)
        }
    };
}