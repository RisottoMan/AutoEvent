// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         RoleCount.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/17/2023 2:44 PM
//    Created Date:     09/17/2023 2:44 PM
// -----------------------------------------

using System.ComponentModel;

namespace AutoEvent.API;

[Description($"Use this to define how many players should be on a team.")]
public class RoleCount
{
    [Description($"The minimum number of players on a team. 0 to ignore.")]
    public int MinimumPlayers { get; set; } = 0;

    [Description($"The maximum number of players on a team. -1 to ignore.")]
    public int MaximumPlayers { get; set; } = -1;

    [Description($"The percentage of players that will be on the team. -1 to ignore.")]
    public float PlayerPercentage { get; set; } = 100;
}