// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SurvivalConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/22/2023 2:13 PM
//    Created Date:     09/22/2023 2:13 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Survival;

public class SurvivalConfig : EventConfig
{
    [Description("How long the round should last in seconds.")]
    public int RoundDurationInSeconds { get; set; } = 300;
    
    [Description("A list of lodaouts players can get.")]
    public List<Loadout> PlayerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunAK,
                ItemType.GunCOM18,
                ItemType.ArmorCombat,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR,
                ItemType.GunCOM18,
                ItemType.ArmorCombat,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunShotgun,
                ItemType.GunCOM18,
                ItemType.ArmorCombat,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
    
    [Description("A list of loadouts zombies can get.")]
    public List<Loadout> ZombieLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scp0492, 100 } },
            Effects = new List<Effect>()
            {
                new Effect() { EffectType = StatusEffect.Disabled },
                new Effect() { EffectType = StatusEffect.Scp1853 },
            },
            Health = 5000,
        }
    };
    
    [Description("The amount of Zombies that can spawn.")]
    public RoleCount Zombies { get; set; } = new RoleCount()
    {
        MinimumPlayers = 1,
        MaximumPlayers = 3,
        PlayerPercentage = 10,
    };

    [Description("The effect that guns should have.")]
    public WeaponEffect WeaponEffect { get; set; } = new WeaponEffect()
    {
        Effects = new List<Effect>()
        {
            new Effect()
            {
                EffectType = StatusEffect.SinkHole,
                Duration = .5f,
                Intensity = 255,
                AddDuration = true,
            },
            new Effect()
            {
                EffectType = StatusEffect.Stained,
                Duration = .5f,
                AddDuration = true,
            }
        }
    };
}