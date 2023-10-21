// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         LavaConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 9:25 PM
//    Created Date:     09/19/2023 9:25 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Infection;

public class LavaConfig : EventConfig
{

    [Description("Can players drop guns.")]
    public bool PlayersCanDropGuns { get; set; } = true;
    [Description("A list of available loadouts that players can get.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } }
        }
    };

    [Description("A list of weapons / items that will spawn around the map, as well as the chance of that object spawning. If you leave this blank, the default map spawns will be used.")]
    public Dictionary<ItemType, float> ItemsAndWeaponsToSpawn { get; set; } = new Dictionary<ItemType, float>()
    {
        { ItemType.GunCOM15,     25 },  // 25 
        { ItemType.GunCOM18,     25 },  // 50
        { ItemType.GunRevolver,  20 },  // 70
        { ItemType.GunFSP9,      13 },  // 83
        { ItemType.GunShotgun,   07 },  // 90
        { ItemType.GunCrossvec,  07 },  // 97
        { ItemType.GunAK,        03 },  // 100
    };
    
    [Description("A list of damage / effects that each gun will give.")]
    public List<WeaponEffect> GunEffects { get; set; } = new List<WeaponEffect>()
    {
        { new WeaponEffect(5,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.1f, true) }, ItemType.GunA7) },
        { new WeaponEffect(5,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.15f, true) }, ItemType.GunAK) },
        { new WeaponEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.1f, true) }, ItemType.GunE11SR) },
        { new WeaponEffect(4,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 3.00f, true) }, ItemType.GunShotgun) },
        { new WeaponEffect(10,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 1.00f, true) }, ItemType.GunCom45) },
        { new WeaponEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.1f, true) }, ItemType.GunCrossvec) },
        { new WeaponEffect(8,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 1.00f, true) }, ItemType.GunCOM15) },
        { new WeaponEffect(8,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 1.00f, true) }, ItemType.GunCOM18) },
        { new WeaponEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.1F, true) }, ItemType.GunLogicer) },
        { new WeaponEffect(12,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 2.00f, true) }, ItemType.GunRevolver) },
        { new WeaponEffect(2,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.03f, true) }, ItemType.GunFRMG0) },
        { new WeaponEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.1f, true) }, ItemType.GunFSP9) },
    };
}
