// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GlassConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 4:33 PM
//    Created Date:     09/18/2023 4:33 PM
// -----------------------------------------

using System.ComponentModel;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Infection;

public class GlassConfig : EventConfig
{
    [Description("Random percent from 40 - 60 will be chosen (ex: 50 percent). 50% of the platforms will be on one side. 50% on the other. This is the Lower Value.")]
    public int MinimumSideOffset { get; set; } = 40;
    [Description("Random percent from 40 - 60 will be chosen (ex: 50 percent). 50% of the platforms will be on one side. 50% on the other. This is the Higher Value.")]
    public int MaximumSideOffset { get; set; } = 60;
    [Description("The method used to randomly scramble the list of platforms.")]
    public SeedMethod PlatformScrambleMethod { get; set; } = SeedMethod.UnityRandom;

    [Description("The salt to use for the cryptographic seed generator.")]
    public string SeedSalt { get; set; } = "salty hasbrown";
    
    [Description("How many seconds until the platforms regenerate. Prevents players from sitting around. -1 to disable.")]
    public float BrokenPlatformRegenerateDelayInSeconds { get; set; } = 20;
}

public enum SeedMethod
{
    UnityRandom,
    SystemRandom,
}