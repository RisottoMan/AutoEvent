// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         LoadoutFlags.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/17/2023 3:32 PM
//    Created Date:     09/17/2023 3:32 PM
// -----------------------------------------

using System;
using PluginAPI.Roles;

namespace AutoEvent.API.Enums;

/// <summary>
/// A list of flags that can be specified while applying a loadout.
/// </summary>
[Flags]
public enum LoadoutFlags
{
    /// <summary>
    /// No flags are specified. Loadouts will be applied normally.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Players will not have a role set.
    /// </summary>
    IgnoreRole = 1,
    
    /// <summary>
    /// Players will not have items added.
    /// </summary>
    IgnoreItems = 2,
    
    /// <summary>
    /// Players will not have their default role items cleared before adding items.
    /// </summary>
    DontClearDefaultItems = 4,

    /// <summary>
    /// Players won't have effects applied to them.
    /// </summary>
    IgnoreEffects = 8,
    
    /// <summary>
    /// Players won't have their health set.
    /// </summary>
    IgnoreHealth = 16,
    
    /// <summary>
    /// Player's won't have their Artificial Health set.
    /// </summary>
    IgnoreAHP = 32,
    
    /// <summary>
    /// Player's won't have their sizes changed. 
    /// </summary>
    IgnoreSize = 64,
    
    /// <summary>
    /// Player's won't have infinite ammo applied from loadouts.
    /// </summary>
    IgnoreInfiniteAmmo = 128,
    
    /// <summary>
    /// Players will have infinite ammo applied. Overrides IgnoreInfiniteAmmo.
    /// </summary>
    ForceInfiniteAmmo = 256,
    
    /// <summary>
    /// Players will note have Godmode set.
    /// </summary>
    IgnoreGodMode = 512,
    
    /// <summary>
    /// Players will not recieve weapons.
    /// </summary>
    IgnoreWeapons = 1024,
    
    /// <summary>
    /// Stamina will not be added.
    /// </summary>
    IgnoreStamina = 2048,
    
    /// <summary>
    /// The player will have an endless amount of ammo.
    /// </summary>
    ForceEndlessClip = 4096,
    
    /// <summary>
    /// The player will stay in the default spawn point.
    /// </summary>
    UseDefaultSpawnPoint = 8192,

    /// <summary>
    /// Only give players items.
    /// </summary>
    ItemsOnly = 16382,
    // 16384
}
