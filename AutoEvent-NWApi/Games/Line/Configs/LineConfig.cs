// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         LineConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 9:29 PM
//    Created Date:     09/19/2023 9:29 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Games.Example;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Infection;

public class LineConfig : EventConfig
{
    [Description("A list of loadouts players can get.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.Scientist, 100 }
            }
        }
    };

    // public DifficultyItem LineDifficulty = new DifficultyItem();
    // public DifficultyItem WallDifficulty = new DifficultyItem();
    // public DifficultyItem DotsDifficulty = new DifficultyItem();
    // public DifficultyItem MiniWallsDifficulty = new DifficultyItem();
    /* todo
     * Eventually I hope to add difficulties to the objects 
     */
}