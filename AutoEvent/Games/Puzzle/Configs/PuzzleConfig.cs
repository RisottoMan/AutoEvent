// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         PuzzleConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 9:42 PM
//    Created Date:     09/19/2023 9:42 PM
// -----------------------------------------

using System.ComponentModel;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Infection;

public class PuzzleConfig : EventConfig
{
    // todo Eventually I want to add difficulties.
    [Description("We intend to rework this config to add difficulties eventually. (This option does nothing)")]
    public bool IgnoreThisConfig { get; set; } = false;
}