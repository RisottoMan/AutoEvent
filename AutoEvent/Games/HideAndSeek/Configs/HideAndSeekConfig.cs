// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         HideAndSeekConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 9:27 PM
//    Created Date:     09/18/2023 9:27 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Infection;

public class HideAndSeekConfig : EventConfig
{
    [Description("The item that the tagged player should get. Do not do Scp018 or Grenades for now. - They will break the event. (working on it - redforce)")]
    public ItemType TaggerWeapon { get; set; } = ItemType.Jailbird;
    [Description("Enables the marshmello effect instead.")]
    public bool HalloweenMelee { get; set; } = true;
    
    [Description("Players who are not the tagger will have the breach scanner effect applied to them, when there are less than or equal to this many non-taggers alive.")]
    public int PlayersRequiredForBreachScannerEffect { get; set; } = 2;

    [Description("The range of the weapon. 0 to disable. [Default: 5]")]
    public float Range { get; set; } = 5f;

    [Description("How long should the tagger get immunity.")]
    public float NoTagBackDuration { get; set; } = 3f;

    [Description("How long players have to tag someone else.")]
    public int TagDuration { get; set; } = 30;
    
    [Description("How long players have to rest before the next tagger group is selected.")]
    public int BreakDuration { get; set; } = 15;

    [Description("The amount of taggers that should spawn.")]
    public RoleCount TaggerCount { get; set; } = new RoleCount(1, 6, 35);

    [Description("A list of loadouts players can get.")]
    public List<Loadout> PlayerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
            Effects = new List<Effect>() { new Effect() { EffectType = StatusEffect.MovementBoost, Intensity = 50, Duration = 0 } },
            Chance = 100,
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    public List<Loadout> TaggerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scientist, 100 } },
            Effects = new List<Effect>() { new Effect() { EffectType = StatusEffect.MovementBoost, Intensity = 70, Duration = 0 } },
            Chance = 100,
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}