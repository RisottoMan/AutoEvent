// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         FriendlyFireSystem.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/26/2023 11:40 PM
//    Created Date:     09/26/2023 11:40 PM
// -----------------------------------------

using System;
using System.Linq;
using System.Reflection;
using CedMod;
using PluginAPI.Core;

namespace AutoEvent;

public class FriendlyFireSystem
{
    public static bool CedModIsPresent { get; private set; }
    public static bool IsFriendlyFireEnabledByDefault { get; set; }
    public static bool FriendlyFireAutoBanDefaultEnabled { get; set; }
    static FriendlyFireSystem()
    {
        CedModIsPresent = false;
        initializeFFSettings();
        FriendlyFireAutoBanDefaultEnabled = IsFriendlyFireEnabledByDefault;
    }
    private static void initializeFFSettings()
    {
        if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.ToLower().Contains("cedmod")))
        {
            DebugLogger.LogDebug("CedMod has been detected.");
            CedModIsPresent = true;
        }
        else
            DebugLogger.LogDebug("CedMod has not been detected.");
    } 

    public static bool FriendlyFireDetectorIsDisabled {
        get
        {
            try
            {
                // if cedmod detector is not paused - false
                if (CedModIsPresent)
                {
                    if (!_cedmodFFAutobanIsDisabled())
                    {
                        return false;
                    }
                }

                // if basegame detector is not paused - false
                return FriendlyFireConfig.PauseDetector;
                // Both MUST be off to be considered "paused".
            }
            catch
            {
                return false;
            }
        }
    }

    private static bool _cedmodFFAutobanIsDisabled()
    {
        return FriendlyFireAutoban.AdminDisabled;
    }

    private static void _cedmodFFDisable()
    {
        FriendlyFireAutoban.AdminDisabled = true;
    }

    private static void _cedmodFFEnable()
    {
        FriendlyFireAutoban.AdminDisabled = false;
    }

    public static void EnableFriendlyFireDetector()
    {
        Log.Debug("Enabling Friendly Fire Detector.");
        try
        {
            FriendlyFireConfig.PauseDetector = false;

            if (CedModIsPresent)
            {
                _cedmodFFEnable();
            }
        }
        catch
        {
            //
        }
    }

    public static void DisableFriendlyFireDetector()
    {
        try
        {
            Log.Debug("Disabling Friendly Fire Detector.");
            FriendlyFireConfig.PauseDetector = true;

            if (CedModIsPresent)
            {
                _cedmodFFDisable();
            }
        }
        catch
        {
            //
        }
    }

    public static void EnableFriendlyFire(/*bool shouldAutobanTeamKills = false*/)
    {
        /*if (shouldAutobanTeamKills && FriendlyFireDetectorIsDisabled)
        {
            EnableFriendlyFireDetector();
        }

        if (!shouldAutobanTeamKills && !FriendlyFireDetectorIsDisabled)
        {
            DisableFriendlyFireDetector();
        }*/

        DebugLogger.LogDebug("Enabling Friendly Fire.");
        Server.FriendlyFire = true;
    }

    public static void DisableFriendlyFire()
    {
        DebugLogger.LogDebug("Disabling Friendly Fire.");
        
        Server.FriendlyFire = false;
        // EnableFriendlyFireDetector();
    }

    public static void RestoreFriendlyFire()
    {
        DebugLogger.LogDebug("Restoring Friendly Fire and Detector.");
        Server.FriendlyFire = IsFriendlyFireEnabledByDefault;
        if (FriendlyFireAutoBanDefaultEnabled && FriendlyFireDetectorIsDisabled)
        {
            EnableFriendlyFireDetector();
        }

        if (!FriendlyFireAutoBanDefaultEnabled && !FriendlyFireDetectorIsDisabled)
        {
            DisableFriendlyFireDetector();
        }
    }
}