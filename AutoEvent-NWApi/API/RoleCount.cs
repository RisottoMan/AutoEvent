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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.API;

[Description($"Use this to define how many players should be on a team.")]
public class RoleCount
{
    public RoleCount() { }

    public RoleCount(int minimumPlayers = 0, int maximumPlayers = -1, float playerPercentage = 100)
    {
        MinimumPlayers = minimumPlayers;
        MaximumPlayers = maximumPlayers;
        PlayerPercentage = playerPercentage;
    }
    [Description($"The minimum number of players on a team. 0 to ignore.")]
    public int MinimumPlayers { get; set; } = 0;

    [Description($"The maximum number of players on a team. -1 to ignore.")]
    public int MaximumPlayers { get; set; } = -1;

    [Description($"The percentage of players that will be on the team. -1 to ignore.")]
    public float PlayerPercentage { get; set; } = 100;

    public List<Player> GetPlayers()
    {
        float percent = Player.GetPlayers().Count * (PlayerPercentage / 100f);
        int players = Mathf.Clamp(Mathf.RoundToInt(percent), MinimumPlayers,
            MaximumPlayers == -1 ? Player.GetPlayers().Count : MaximumPlayers);
        List<Player> validPlayers = new List<Player>();
        try
        {
            for (int i = 0; i < players; i++)
            {

                Player ply = Player.GetPlayers().Where(x => !validPlayers.Contains(x)).ToList().RandomItem();
                validPlayers.Add(ply);
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("Could not assign player to list.", LogLevel.Warn);
            DebugLogger.LogDebug($"{e}", LogLevel.Debug);
        }
        return validPlayers;
    }
}