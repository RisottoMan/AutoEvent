// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         RoundEndArgs.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/06/2023 12:32 PM
//    Created Date:     09/06/2023 12:32 PM
// -----------------------------------------

using System;
using System.Linq;
using PlayerRoles;
using PluginAPI.Core;

namespace AutoEvent.Interfaces;

public static class EndConditions
{
    /// <summary>
    /// Used to check if the round can end. Checks if more than one player is alive.
    /// </summary>
    /// <param name="countScpsAsPlayers">If true, scps are included in the class to be counted. If false, scp's won't be counted.</param>
    /// <returns>True if only one player is alive, False if more than one player is alive</returns>
    public static bool OnePlayerAlive(bool countScpsAsPlayers = false) => MoreThanXPlayersAlive(1, countScpsAsPlayers);

    /// <summary>
    /// Used to check if the round can end. Checks if any players are alive.
    /// </summary>
    /// <param name="countScpsAsPlayers">If true, scps are included in the class to be counted. If false, scp's won't be counted.</param>
    /// <returns>True if no players are alive, False if players are alive</returns>
    public static bool NoPlayersAlive(bool countScpsAsPlayers = false) => MoreThanXPlayersAlive(0, countScpsAsPlayers);
    
    /// <summary>
    /// Used to check if the round can end. Checks if any players are alive.
    /// </summary>
    /// <param name="playersToSearchFor">How many players can be alive that are not counted.</param>
    /// <param name="countScpsAsPlayers">If true, scps are included in the class to be counted. If false, scp's won't be counted.</param>
    /// <returns>True if more than X players are alive, Otherwise false</returns>
    public static bool MoreThanXPlayersAlive(int playersToSearchFor, bool countScpsAsPlayers = false)
    {
        return Player.GetPlayers().Count(x => x.IsAlive && (countScpsAsPlayers || !x.IsSCP)) > playersToSearchFor;
    }

    /// <summary>
    /// Used to check if the round can end. Checks how many players are alive of a give role.
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <param name="playersToSearchFor">How many players can be alive that are not counted.</param>
    /// <returns>True if more than X players of the role are alive, Otherwise false</returns>
    public static bool RoleHasMoreThanXPlayers(RoleTypeId role, int playersToSearchFor = 0)
    {
        return Player.GetPlayers().Count(x => x.Role == role) > playersToSearchFor;
    }
    
    /// <summary>
    /// Used to check if the round can end. Checks how many players are alive of a give role.
    /// </summary>
    /// <param name="team">The team to check</param>
    /// <param name="playersToSearchFor">How many players can be alive that are not counted.</param>
    /// <returns>True if more than X players of the team are alive, Otherwise false</returns>
    public static bool TeamHasMoreThanXPlayers(Team team, int playersToSearchFor = 0)
    {
        return Player.GetPlayers().Count(x => x.Team == team) > playersToSearchFor;
    }

    
    public static bool TimeHasRunOut(this Event @event, int timeInSeconds)
    {
        //return @event.StartTime.Subtract(DateTime.UtcNow).TotalSeconds >= timeInSeconds;
        return @event.EventTime.TotalSeconds >= timeInSeconds;
    }
    
}