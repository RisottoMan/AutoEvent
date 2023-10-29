// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         KnivesConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 6:03 PM
//    Created Date:     09/19/2023 6:03 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Infection;

public class KnivesConfig : EventConfig
{
    [Description("Enables the halloween effect melee instead of jailbird.")]
    public bool HalloweenMelee { get; set; } = true;
    
    [Description("A list of loadouts that players on team 1 can get.")]
    public List<Loadout> Team1Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfCaptain, 100 } },
            
        }
    };

    [Description("A list of loadouts that players on team 2 can get.")]
    public List<Loadout> Team2Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ChaosRepressor, 100 } },
            
        }

    };
}