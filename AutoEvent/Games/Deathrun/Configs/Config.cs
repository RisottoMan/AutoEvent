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
    public int RoundDurationInMinutes { get; set; } = 5;
}