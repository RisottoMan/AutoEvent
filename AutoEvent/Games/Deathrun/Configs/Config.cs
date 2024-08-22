using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Deathrun;
public class Config : EventConfig
{
    [Description("How long the round should last in minutes.")]
    public int RoundDurationInSeconds { get; set; } = 300;
    [Description("How many seconds after the start of the game can be given a second life? Disable -> -1")]
    public int SecondLifeInSeconds { get; set; } = 15;
}