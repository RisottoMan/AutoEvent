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
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.GhostBusters.Configs;

public class GhostBustersConfig : EventConfig
{
    [Description("How much time the hunters have to kill the ghosts until ghosts can kill them.")]
    public int TimeUntilMidnightInSeconds { get; set; } = 260;

    [Description("How much time the ghosts have to kill the hunters, after midnight hits.")]
    public int MidnightDurationInSeconds { get; set; } = 120;

    public float MicroRechargePercentPerSecond { get; set; } = 10;
    public float MicroRechargeDelayOffset { get; set; } = 5;

    public RoleCount HunterCount { get; set; } = new RoleCount(2, 4, 30);

    [Description("Recommended to use Metal or Ghostly effects, as they are the most fitting. Ghostly can be too hard to see.")]
    public List<Loadout> GhostLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.Scientist, 100 }
            },
            Effects = new List<Effect>()
            {
                new Effect()
                {
                    EffectType = StatusEffect.Metal
                },
            },
        }
    };
    public Loadout SniperLoadout { get; set; } = new Loadout()
    {
        Roles = new Dictionary<RoleTypeId, int>()
        {
            { RoleTypeId.ChaosRifleman, 100 }
        },
        Items = new List<ItemType>()
        {
            ItemType.ParticleDisruptor,
            ItemType.Radio
        },
        Health = 2000,
    };
    public Loadout TankLoadout { get; set; } = new Loadout()
    {
        Roles = new Dictionary<RoleTypeId, int>()
        {
            { RoleTypeId.ChaosMarauder, 100 }
        },
        Items = new List<ItemType>()
        {
            ItemType.MicroHID,
            ItemType.Radio
        },
        Health = 2000,
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
            ItemType.Radio
        },
        Effects = new List<Effect>()
        {
            new Effect()
            {
                EffectType = StatusEffect.MovementBoost,
                Intensity = 20,
            }
        },
        Health = 2000,
    };
    
}