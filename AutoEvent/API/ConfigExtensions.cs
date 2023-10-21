// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ConfigApi.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/01/2023 1:19 PM
//    Created Date:     10/01/2023 1:19 PM
// -----------------------------------------

using System;
using System.Linq;
using System.Reflection;
using AutoEvent.Interfaces;

namespace AutoEvent.API;

public static class ConfigExtensions
{
    /// <summary>
    /// Sets the configs for an event.
    /// </summary>
    /// <param name="ev">The event to set the config.</param>
    /// <param name="conf">The config to set.</param>
    /// <returns>True if successfully set, false if an error occured.</returns>
    public static bool SetConfig(this Event ev, EventConfig conf)
    {
        bool failed = false;
        foreach (PropertyInfo property in ev.GetType().GetProperties())
        {
            if (property.GetCustomAttribute<EventConfigAttribute>() is null)
                continue;
            if (property.GetCustomAttribute<EventConfigPresetAttribute>() is not null)
                continue;
            if (property.PropertyType != conf.GetType() && property.PropertyType.BaseType != conf.GetType())
                continue;
            DebugLogger.LogDebug($"[{conf.PresetName}->{property.Name}] Property: {property.PropertyType?.Name} PropBase: {property.PropertyType?.BaseType?.Name}, Conf: {conf.GetType()?.Name}, ConfBase: {conf.GetType().BaseType?.Name}");
            try
            {
                property.SetValue(ev, conf);
            }
            catch (Exception e)
            {
                failed = true;
                DebugLogger.LogDebug($"Could not set value of property while changing presets. \n{e}");
            }
        }

        return !failed;
    }

    /// <summary>
    /// Tries to get the name of a preset.
    /// </summary>
    /// <param name="ev">The event to search for.</param>
    /// <param name="searchName">The name of the preset you are searching for.</param>
    /// <param name="presetName">The full name of the preset.</param>
    /// <returns>True if a preset is found, false if the preset could not be found.</returns>
    public static bool TryGetPresetName(this Event ev, string searchName, out string presetName)
    {
        var conf = ev.ConfigPresets.FirstOrDefault(x => x.PresetName.ToLower() == searchName.ToLower());
        if (conf is not null)
        {
            presetName = conf.PresetName;
            return true;
        }

        presetName = "Preset Not Found";
        return false;
    }
    /// <summary>
    /// Sets the configs for an event.
    /// </summary>
    /// <param name="ev">The event to set the config.</param>
    /// <param name="confName">The name of the config to search for and set.</param>
    /// <returns>True if successfully set, false if an error occured.</returns>
    public static bool SetConfig(this Event ev, string confName)
    {
        // Config doesnt exist.
        var conf = ev.ConfigPresets.FirstOrDefault(x => x.PresetName.ToLower() == confName.ToLower());
        if (conf is null)
        {
            return false;
        }

        return SetConfig(ev, conf);
    }
}