// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Loadout.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/17/2023 2:51 PM
//    Created Date:     09/17/2023 2:51 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API.Enums;
using Exiled.API.Features.Items;
using PlayerRoles;
using PluginAPI.Enums;
using UnityEngine;

namespace AutoEvent.API;

[Description("A loadout that a user can get during the event")]
public class Loadout
{

    [Description("A list of roles, and the chance of getting the role.")]
    public Dictionary<RoleTypeId, int> Roles { get; set; } = new Dictionary<RoleTypeId, int>();

    [Description("The items that this class spawns with.")]
    public List<ItemType> Items { get; set; } = new List<ItemType>();

    [Description("A list of effects the player will spawn with.")]
    public List<Effect> Effects { get; set; } = new List<Effect>();
    
    [Description("The health that this class has. 0 is default role health. -1 is godmode.")]
    public int Health { get; set; } = 0;
    
    [Description("How much artificial health the class has. 0 is default artificial health.")]
    public int ArtificialHealth { get; set; } = 0;

    [Description("The chance of a user getting this class. Chance cannot be <= 0, it will be set to 1.")]
    public int Chance { get; set; } = 1;

    [Description("The size of this class. One is normal.")]
    public Vector3 Size { get; set; } = Vector3.one;

    [Description("The stamina the player should get. 0 will ignore.")]
    public float Stamina { get; set; } = 0f;

    [Description("Should the player have infinite ammo.")]
    public AmmoMode InfiniteAmmo { get; set; } = AmmoMode.None;
}

public enum AmmoMode
{
    [Description("Ammo will be normal.")]
    None = 0 ,
    [Description("Player will be able to reload regardless of if there is ammo in their inventory.")]
    InfiniteAmmo = 2,
    [Description("The player will be able to fire indefinitely.")]
    EndlessClip = 3,
}