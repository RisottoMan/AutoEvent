// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         PlatformSelector.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 2:50 PM
//    Created Date:     10/16/2023 2:50 PM
// -----------------------------------------

using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.RNG;
using AutoEvent.Games.Infection;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Glass.Features;

public class PlatformSelector
{
    public int PlatformCount { get; set; }
    internal string Seed { get; set; }
    internal List<PlatformData> PlatformData { get; set; }
    public int MinimumSideOffset { get; set; } = 0;
    public int MaximumSideOffset { get; set; } = 0;
    public int LeftSidedPlatforms { get; set; } = 0;
    public int RightSidedPlatforms { get; set; } = 0;
    private SeedMethod _seedMethod;

    public PlatformSelector(int platformCount, string salt, int minimumSideOffset, int maximumSideOffset,
        SeedMethod seedMethod)
    {
        PlatformCount = platformCount;
        PlatformData = new List<PlatformData>();
        MinimumSideOffset = minimumSideOffset;
        MaximumSideOffset = maximumSideOffset;
        _seedMethod = seedMethod;
        initSeed(salt);
        _selectPlatformSideCount();
        _createPlatforms();
        _logOutput();
    }

    private void initSeed(string salt)
    {
        var bytes = RNGGenerator.GetRandomBytes().AddRangeToArray(RNGGenerator.TextToBytes(salt));
        Seed = RNGGenerator.GetSeed(bytes);
    }

    private void _selectPlatformSideCount()
    {
        bool leftSidePriority = true;
        int seedInt = RNGGenerator.GetIntFromSeededString(Seed, 3, 4, 0);
        int percent = 50;
        int priority = 5;
        int remainder = 5;
        switch (_seedMethod)
        {
            case SeedMethod.UnityRandom:
                UnityEngine.Random.InitState(seedInt);
                leftSidePriority = UnityEngine.Random.Range(0, 2) == 1;
                percent = UnityEngine.Random.Range((int)MinimumSideOffset, (int)MaximumSideOffset);
                break;
            case SeedMethod.SystemRandom:
                var random = new System.Random(seedInt);
                leftSidePriority = random.Next(0, 2) == 1;
                percent = random.Next((int)MinimumSideOffset, (int)MaximumSideOffset);
                random.Next();
                break;
        }

        priority = (int)((float)PlatformCount * ((float)percent / 100f));
        remainder = PlatformCount - priority;
        LeftSidedPlatforms = leftSidePriority ? priority : remainder;
        RightSidedPlatforms = leftSidePriority ? remainder : priority;
    }

    private void _createPlatforms()
    {
        List<PlatformData> data = new List<PlatformData>();
        for (int i = 0; i < LeftSidedPlatforms; i++)
        {
            data.Add(new PlatformData(true, RNGGenerator.GetIntFromSeededString(Seed, 4, 4, 1 + i)));
        }

        for (int i = 0; i < RightSidedPlatforms; i++)
        {
            data.Add(new PlatformData(false,
                RNGGenerator.GetIntFromSeededString(Seed, 4, 4, 1 + i + LeftSidedPlatforms)));
        }

        PlatformData = data.OrderBy(x => x.Placement).ToList();
    }

    private void _logOutput()
    {
        DebugLogger.LogDebug(
            $"Selecting {PlatformCount} Platforms. [{MinimumSideOffset}, {MaximumSideOffset}]   {LeftSidedPlatforms} | {RightSidedPlatforms}  Seed: {Seed}",
            LogLevel.Debug, false);
        foreach (var platform in PlatformData.OrderByDescending(x => x.Placement))
        {
            DebugLogger.LogDebug(
                (platform.LeftSideIsDangerous ? "[X] [=]" : "[=] [X]") + $"  Priority: {platform.Placement}",
                LogLevel.Debug, false);
        }
    }
}