// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SpleefTranslation.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 6:20 PM
//    Created Date:     10/17/2023 6:20 PM
// -----------------------------------------

namespace AutoEvent.Games.Spleef.Configs;

public class SpleefTranslation
{
    public string SpleefCommandName { get; set; } = "spleef";
    public string SpleefName { get; set; } = "Spleef";
    public string SpleefDescription { get; set; } = "Break the platforms below your enemies before you fall into the void!";
    public string SpleefStart { get; set; } = "<color=red>Starts in: </color>{time}";
    public string SpleefAllDied { get; set; } = "<color=red>All players died</color>\nMini-game ended";
    public string SpleefSeveralSurvivors { get; set; } = "<color=red>Several people survived</color>\nMini-game ended";
    public string SpleefWinner { get; set; } = "<color=red>Winner: {winner}</color>\nMini-game ended";
    public string SpleefDied { get; set; } = "<color=red>Burned in Lava</color>";
}