// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GhostBustersConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 2:05 AM
//    Created Date:     10/28/2023 2:05 AM
// -----------------------------------------

using System.Collections.Generic;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.GhostBusters.Configs;

public class GhostBustersConfig : EventConfig
{
    public Loadout SniperLoadout { get; set; } = new Loadout()
    {
        Roles = new Dictionary<RoleTypeId, int>()
        {
            { RoleTypeId.ChaosRifleman, 100 }
        },
        Items = new List<ItemType>()
        {
            ItemType.ParticleDisruptor
        }
    };
    public Loadout TankLoadout { get; set; } = new Loadout()
    {
        Roles = new Dictionary<RoleTypeId, int>()
        {
            { RoleTypeId.ChaosMarauder, 100 }
        },
        Items = new List<ItemType>()
        {
            ItemType.MicroHID
        },
    };
    public Loadout MeleeLoadout { get; set; } = new Loadout()
    {
        Roles = new Dictionary<RoleTypeId, int>()
        {
            { RoleTypeId.ChaosRifleman, 100 }
        },
        Items = new List<ItemType>()
        {
            ItemType.Jailbird, 
        },
        Effects = new List<Effect>()
        {
            new Effect()
            {
                EffectType = StatusEffect.MovementBoost,
                Intensity = 20,
            }
        }
    };
    
}