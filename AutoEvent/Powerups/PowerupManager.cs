// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         PowerupManager.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 1:05 AM
//    Created Date:     10/17/2023 1:05 AM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.Interfaces;
using MEC;
using PluginAPI.Core;

namespace AutoEvent;

public class PowerupManager
{
    public static PowerupManager Singleton;
    internal PowerupManager()
    {
        Singleton = this;
        RegisteredPowerups = AbstractedTypeExtensions.InstantiateAllInstancesOfType<Powerup>();
        PowerupCoroutineHandle = Timing.RunCoroutine(ProcessPowerups(), "Powerup Processing Coroutine");
    }
    private CoroutineHandle PowerupCoroutineHandle { get; set; }
    private bool KillLoop = false;
    internal void KillPowerupManager()
    {
        KillLoop = true;
        Timing.CallDelayed(PowerupFrameDelay + 3, () =>
        {
            if (PowerupCoroutineHandle.IsRunning)
            {
                Timing.KillCoroutines(PowerupCoroutineHandle);
            }
        });
    }
    private const int PowerupFrameDelay = 3;
    
    private IEnumerator<float> ProcessPowerups()
    {
        while (!KillLoop)
        {
            if (AutoEvent.ActiveEvent == null)
            {
                goto skip;
            }

            foreach (Powerup powerup in RegisteredPowerups)
            {
                if (powerup.ActivePlayers.Count > 0)
                {
                    Dictionary<Player, float> newKeys = new Dictionary<Player, float>();
                    foreach (var kvp in powerup.ActivePlayers)
                    {
                        float newDuration = kvp.Value - PowerupFrameDelay;
                        if (newDuration <= 0)
                        {
                            powerup.ExpirePowerup(kvp.Key);
                            continue;
                        }
                        newKeys.Add(kvp.Key, newDuration);
                    }
                    powerup.ActivePlayers = newKeys;
                }
            }
            
            skip:
            yield return Timing.WaitForSeconds(PowerupFrameDelay);
        }
    }

    public static List<Powerup> RegisteredPowerups { get; set; }

    public static T GetPowerup<T>() where T : Powerup
    {
        return (T)RegisteredPowerups.First(powerup => powerup is T);
    }

    public static Powerup? GetPowerup(Type type)
    {
        if (type.BaseType != typeof(Powerup))
        {
            return null;
        }

        return RegisteredPowerups.FirstOrDefault(powerup => powerup.GetType() == type);
    }

    public static Powerup? GetPowerup(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return RegisteredPowerups.FirstOrDefault(powerup => powerup.Name.ToLower().Contains(name.ToLower()));
    }
}