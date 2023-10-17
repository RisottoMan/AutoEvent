// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GridPoint.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 4:46 PM
//    Created Date:     10/16/2023 4:46 PM
// -----------------------------------------

namespace AutoEvent.Games.Puzzle;

public struct GridPoint
{
    public GridPoint(byte intensity, byte chance)
    {
        Intensity = intensity;
        Chance = chance;
    }
    public byte Intensity { get; init; }
    public byte Chance { get; init; }
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
}