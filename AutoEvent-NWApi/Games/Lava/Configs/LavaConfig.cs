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
    public Dictionary<ItemType, GunEffect> GunEffects { get; set; } = new Dictionary<ItemType, GunEffect>()
    {
        { ItemType.GunA7,        new GunEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.50f, true) }) },
        { ItemType.GunAK,        new GunEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.50f, true) }) },
        { ItemType.GunE11SR,     new GunEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.50f, true) }) },
        { ItemType.GunShotgun,   new GunEffect(30,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 5.00f, true) }) },
        { ItemType.GunCom45,     new GunEffect(10,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 1.00f, true) }) },
        { ItemType.GunCrossvec,  new GunEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.50f, true) }) },
        { ItemType.GunCOM15,     new GunEffect(20,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 1.00f, true) }) },
        { ItemType.GunCOM18,     new GunEffect(20,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 1.00f, true) }) },
        { ItemType.GunLogicer,   new GunEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.50f, true) }) },
        { ItemType.GunRevolver,  new GunEffect(20,   new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 1.00f, true) }) },
        { ItemType.GunFRMG0,     new GunEffect(2,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.50f, true) }) },
        { ItemType.GunFSP9,      new GunEffect(3,    new List<Effect>(){ new Effect(StatusEffect.Concussed, 1, 0.50f, true) }) },
    };
}
