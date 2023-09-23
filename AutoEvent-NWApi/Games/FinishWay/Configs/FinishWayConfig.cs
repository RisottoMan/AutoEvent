// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         FinishWayConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 4:08 PM
//    Created Date:     09/18/2023 4:08 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Infection;

public class FinishWayConfig : EventConfig
{
    [Description("How long the event should last in seconds. [Default: 60]")]
    public int EventDurationInSeconds = 60;

    [Description("A list of loadouts players can get.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Tutorial, 100 } }
        }
    };
}