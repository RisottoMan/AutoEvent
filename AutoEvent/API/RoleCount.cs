using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Features;
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

    public List<Player> GetPlayers(bool alwaysLeaveOnePlayer = true, List<Player>? availablePlayers = null)
    {
        float percent = Player.List.Count * (PlayerPercentage / 100f);
        int players = Mathf.Clamp((int)percent, MinimumPlayers,
            MaximumPlayers == -1 ? Player.List.Count : MaximumPlayers);
        List<Player> validPlayers = new List<Player>();
        // DebugLogger.LogDebug($"Selecting Players: {players} < {(int)percent:F2} ({percent}) <  ");
        try
        {
            for (int i = 0; i < players; i++)
            {
                List<Player> playersToPullFrom = (availablePlayers ?? Player.List) .Where(x => !validPlayers.Contains(x)).ToList();
                if (playersToPullFrom.Count < 1)
                {
                    DebugLogger.LogDebug("Cannot pull more players.");
                    break;
                }

                if (playersToPullFrom.Count < 2)
                {
                    DebugLogger.LogDebug("Only one more player available. Pulling that player.");
                    validPlayers.Add(playersToPullFrom[0]);
                    break;
                }
                int rndm = UnityEngine.Random.Range((int)0, (int)playersToPullFrom.Count);
                
                Player ply = playersToPullFrom[rndm];
                validPlayers.Add(ply);
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("Could not assign player to list.", LogLevel.Warn);
            DebugLogger.LogDebug($"{e}");
        }
        if(alwaysLeaveOnePlayer && validPlayers.Count >= (availablePlayers ?? Player.List).Count)
        {
            var plyToRemove = validPlayers.RandomItem();
            validPlayers.Remove(plyToRemove);
        }
        return validPlayers;
    }
}