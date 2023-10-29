// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         FallDownConfigPresets.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 11:18 PM
//    Created Date:     10/16/2023 11:18 PM
// -----------------------------------------

namespace AutoEvent.Games.Infection;

public static class FallDownConfigPresets
{
    public static FallDownConfig PlatformWarning => new FallDownConfig()
    {
        PlatformsHaveColorWarning = true
    };
}