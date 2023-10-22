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
using MEC;
using PluginAPI.Core;
using Powerups.Extensions;

namespace Powerups;
 
public sealed class PowerupManager
{
    /// <summary>
    /// The main instance of the powerup manager. You must call <see cref="API.Initialize"/> or else the singleton will be null.
    /// </summary>
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

    /// <summary>
    /// A list of all registered powerups.
    /// </summary>
    public static List<Powerup> RegisteredPowerups { get; set; }

    /// <summary>
    /// Gets a <see cref="Powerup"/> by its type.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Powerup"/> you are trying to get.</typeparam>
    /// <returns>The <see cref="Powerup"/> (if it is found) or null.</returns>
    public static T GetPowerup<T>() where T : Powerup
    {
        return (T)RegisteredPowerups.First(powerup => powerup is T);
    }

    /// <summary>
    /// Gets a <see cref="Powerup"/> by its type.
    /// </summary>
    /// <param name="type">The type of the <see cref="Powerup"/> you are trying to get.</param>
    /// <returns>The <see cref="Powerup"/> (if it is found) or null.</returns>
    public static Powerup? GetPowerup(Type type)
    {
        if (type.BaseType != typeof(Powerup))
        {
            return null;
        }

        return RegisteredPowerups.FirstOrDefault(powerup => powerup.GetType() == type);
    }

    /// <summary>
    /// Gets a <see cref="Powerup"/> by its name.
    /// </summary>
    /// <param name="name">The name of the <see cref="Powerup"/> you are trying to get.</param>
    /// <returns>The <see cref="Powerup"/> (if it is found) or null.</returns>
    public static Powerup? GetPowerup(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return RegisteredPowerups.FirstOrDefault(powerup => powerup.Name.ToLower().Contains(name.ToLower()));
    }
}