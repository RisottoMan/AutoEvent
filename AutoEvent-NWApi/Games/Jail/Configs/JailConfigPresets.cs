// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         JailConfigPresets.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/24/2023 1:48 PM
//    Created Date:     09/24/2023 1:48 PM
// -----------------------------------------

namespace AutoEvent.Games.Infection;

public static class JailConfigPresets
{
    public static JailConfig AdminEvent => new JailConfig()
    {
        LockdownSettings = new LockdownSettings()
        {
            AutoReleaseDelayInSeconds = -1,
            LockdownDurationInSeconds = -1,
            LockdownCooldownDurationInSeconds = -1,
            LockdownLocksGatesAsWell = false,
        },
        PrisonerLives = 1,
    };

    public static JailConfig PublicServerEvent => new JailConfig()
    {
        LockdownSettings = new LockdownSettings(20f, 15f, 15f, true),
        PrisonerLives = 3,
    };
}