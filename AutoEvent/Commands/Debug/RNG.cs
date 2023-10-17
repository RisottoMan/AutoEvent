// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ReloadConfigs.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 4:29 PM
//    Created Date:     09/13/2023 4:29 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoEvent.Games.Glass;
using AutoEvent.Games.Glass.Features;
using AutoEvent.Games.Infection;
using AutoEvent.Games.Puzzle;
using AutoEvent.Interfaces;
using CommandSystem;
using HarmonyLib;
using PluginAPI.Core;
#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#endif
namespace AutoEvent.Commands.Debug;


public class RNG : ICommand, IPermission
{
    public string Command => nameof(RNG);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Creates an RNG platform list.";

    public string Permission { get; set; } = "ev.debug";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count > 0)
        {
            switch (arguments.At(0).ToLower())
            {
                case "platform" or "glass":
                    goto platform;
                case "puzzle" or "grid":
                    goto grid;
            }
        }

        response = "You must specify a valid rng usage.";
        return false;
        
        grid:
        byte platformSpread = 1;
        byte sizeX = 5;
        byte sizeY = 5;
        SeedMethod method = SeedMethod.UnityRandom;
        string salt = "salty hashbrown";
        for (int i = 1; i < arguments.Count; i++)
        {
            switch (i - 1)
            {
                case 0:
                    sizeX = byte.Parse(arguments.At(i));
                    break;
                case 1:
                    sizeY = byte.Parse(arguments.At(i));
                    break;
                case 2:
                    platformSpread = byte.Parse(arguments.At(i));
                    break;
                case 3:
                    if (arguments.At(i).ToLower() == "unity")
                        method = SeedMethod.UnityRandom;
                    else
                        method = SeedMethod.SystemRandom;
                    break;
                case 4:
                    salt = arguments.At(i);
                    break;
                default:
                    salt += arguments.At(i);
                    break;
            }
        }
        DebugLogger.LogDebug($"Generating Grid Selector. Settings: Size: {sizeX}x{sizeY}, (salt: \"{salt}\"), Method: {method}");
        var selector = new GridSelector(sizeX, sizeY, salt, method);
        DebugLogger.LogDebug($"Selecting Grid Item. Platform Spread: {platformSpread}");
        selector.SelectGridItem(platformSpread: platformSpread);
        response = "Output to console successfully.";
        return true;
        
        
        
        
        platform:
        int platformCount = 10;
        SeedMethod seedMethod = SeedMethod.UnityRandom;
        int minimumSideOffset = 40;
        int maximumSideOffset = 60;
        salt = "salty hashbrown";
        for (int i = 1; i < arguments.Count; i++)
        {
            switch (i - 1)
            {
                case 0:
                    platformCount = int.Parse(arguments.At(i));
                    break;
                case 1:
                    if (arguments.At(i).ToLower() == "unity")
                        seedMethod = SeedMethod.UnityRandom;
                    else
                        seedMethod = SeedMethod.SystemRandom;
                    break;
                case 2:
                    minimumSideOffset = int.Parse(arguments.At(i));
                    break;
                case 3:
                    maximumSideOffset = int.Parse(arguments.At(i));
                    break;
                case 4:
                    salt = arguments.At(i);
                    break;
                default:
                    salt += arguments.At(i);
                    break;
            }
        }
        PlatformSelector platformSelector = new PlatformSelector(platformCount, salt, minimumSideOffset, maximumSideOffset, seedMethod);

        response = $"Selecting {platformSelector.PlatformCount} Platforms. [{platformSelector.MinimumSideOffset}, {platformSelector.MaximumSideOffset}]   {platformSelector.LeftSidedPlatforms} | {platformSelector.RightSidedPlatforms}  Seed: {platformSelector.Seed}\n";
        foreach (var platform in platformSelector.PlatformData.OrderByDescending(x => x.Placement))
        {
            if (IsConsoleCommandSender)
                response += (platform.LeftSideIsDangerous ? "[X] [=]" : "[=] [X]") + $"  Priority: {platform.Placement}\n";
            else
                response += (platform.LeftSideIsDangerous ? "<color=red>[X]<color=white> [=]" : "[=] <color=red>[X]<color=white>") + $"  Priority: <color=yellow>{platform.Placement}\n";
        }

        return true;
    }
}