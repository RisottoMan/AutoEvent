// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         JailLockdownSystem.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/24/2023 1:22 PM
//    Created Date:     09/24/2023 1:22 PM
// -----------------------------------------

using AutoEvent.Games.Infection;
using UnityEngine;

namespace AutoEvent.Games.Jail;

public class JailLockdownSystem
{
    public JailLockdownSystem(Plugin plugin)
    {
        _plugin = plugin;
        LockDownRemainingDuration = Config.LockdownSettings.AutoReleaseDelayInSeconds;
    }

    private Plugin _plugin;
    private Infection.Config Config => _plugin.Config;
    private GameObject PrisonerDoors => _plugin.PrisonerDoors;
    internal bool LockDownActive => !_plugin.PrisonerDoors.GetComponent<JailerComponent>().IsOpen;
    internal float LockDownCooldown { get; set; }
    internal float LockDownRemainingDuration { get; set; }

    internal void ProcessTick(bool isCountdownTick = false)
    {
        if (LockDownCooldown > 0)
        {
            LockDownCooldown -= 1f;
        }
            
        if (LockDownRemainingDuration > 0)
        {
            LockDownRemainingDuration -= 1f;
        }

        // only toggle lockdown if zero. Then set to -1 to disable retriggering.
        if (LockDownRemainingDuration == 0)
        {
            ToggleLockdown();
            LockDownRemainingDuration = -1;
        }
    }

    internal bool ToggleLockdown(BypassLevel bypassLevel = BypassLevel.None)
    {
        // If a lockdown is triggered, release it and add a cooldown.
        if (LockDownActive)
        {
            DebugLogger.LogDebug("Releasing Lockdown.");
            PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
            LockDownCooldown = Config.LockdownSettings.LockdownCooldownDurationInSeconds;
            LockDownRemainingDuration = 0;
            return true;
        }

        if (bypassLevel == BypassLevel.BypassMode)
        {
            DebugLogger.LogDebug("Bypass Mode - Skipping Cooldown. Duration will be maximum.");
            PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
            LockDownRemainingDuration = float.MaxValue;
            return true;
        }

        // If cooldown is not done, wait.
        if (!LockDownActive && LockDownCooldown > 0)
        {
            DebugLogger.LogDebug("Cooldown is not finished. Cannot trigger lockdown.");
            return false;
        }

        // Allow Locking Down Doors
        if (!LockDownActive && LockDownCooldown <= 0)
        {
            DebugLogger.LogDebug("Cooldown is finished, Triggering lockdown.");
            PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
            LockDownCooldown = 0;
            LockDownRemainingDuration = Config.LockdownSettings.LockdownDurationInSeconds * (int)bypassLevel;
            return true;
        }

        DebugLogger.LogDebug("Error lockdown trigger somehow failed.");
        return false;
    }
}