// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ConfigPresets.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 11:14 PM
//    Created Date:     10/16/2023 11:14 PM
// -----------------------------------------

namespace AutoEvent.Games.Puzzle;

public static class PuzzleConfigPresets
{
    public static PuzzleConfig ColorMatch { get; set; } = new PuzzleConfig()
    {
        UseRandomPlatformColors = true,
    };
    public static PuzzleConfig Run { get; set; } = new PuzzleConfig()
    {
        PlatformsOnEachAxis = 10,
        UseRandomPlatformColors = false,
    };
}