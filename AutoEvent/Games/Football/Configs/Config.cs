using System.ComponentModel;
using AutoEvent.Interfaces;
using UnityEngine;

namespace AutoEvent.Games.Football;
public class Config : EventConfig
{
    [Description("How many points a team needs to get to win. [Default: 3]")]
    public int PointsToWin { get; set; } = 3;
    [Description("How long the match should take in seconds. [Default: 180]")]
    public int MatchDurationInSeconds { get; set; } = 180;
}