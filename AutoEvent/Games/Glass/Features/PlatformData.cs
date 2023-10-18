// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         PlatformData.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 2:49 PM
//    Created Date:     10/16/2023 2:49 PM
// -----------------------------------------

namespace AutoEvent.Games.Glass.Features;

public struct PlatformData
{
    public PlatformData(bool leftSideIsDangerous, int placement)
    {
        LeftSideIsDangerous = leftSideIsDangerous;
        Placement = placement;
    }
    public int Placement { get; set; }
    public bool LeftSideIsDangerous { get; set; }
    public bool RightSideIsDangerous
    {
        get => !LeftSideIsDangerous;
        set => LeftSideIsDangerous = !value;
    }
}