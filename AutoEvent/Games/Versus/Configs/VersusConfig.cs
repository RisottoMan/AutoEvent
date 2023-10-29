// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         VersusConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/22/2023 2:13 PM
//    Created Date:     09/22/2023 2:13 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Versus;

public class VersusConfig : EventConfig
{
    [Description("Can be used to disable the jailbird charging attack.")]
    public bool JailbirdCanCharge { get; set; } = false;
    
    [Description("How long to wait before forcefully selecting a random player. Set to -1 to disable.")]
    public int AutoSelectDelayInSeconds { get; set; } = 10;
    
    [Description("Enables the halloween effect melee instead of jailbird.")]
    public bool HalloweenMelee { get; set; } = true;
    
    [Description("Loadouts for team 1.")]
    public List<Loadout> Team1Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scientist, 100 } },
        }
    };
    [Description("Loadouts for team 2.")]
    public List<Loadout> Team2Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
        }
    };
}