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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#endif
namespace AutoEvent.Commands.Config;


public class Select : ICommand, IUsageProvider, IPermission
{
    public string Command => nameof(Select);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Selects a config preset to use for an event.";
    public string[] Usage => new[] { "[Event]", "[Preset / Default]" };
    public string Permission { get; set; } = "ev.config.select";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        
        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }
        
        // Event Missing
        if (arguments.Count < 1)
        {
            response = $"Please select an event to change presets.\n";
            goto BasicUsage;
        }

        // Event Not Found
        Event ev = Event.GetEvent(arguments.At(0));
        if (ev is null)
        {
            DebugLogger.LogDebug($"Event is null.", LogLevel.Debug);
            response = $"Could not find event \"{arguments.At(0)}\".";
            return false;
        }
        
        // Preset Missing
        if (arguments.Count < 2)
        {
            response = $"Please select a preset to use.\n";
            goto BasicUsage;
        }
        
        // Preset List is null - this shouldn't be possible.
        if (ev.ConfigPresets is null)
        {
            DebugLogger.LogDebug($"Config Presets List is null.", LogLevel.Debug);
            response = "An error has occured.";
            return false;
        }

        // Config doesnt exist.
        var conf = ev.ConfigPresets.FirstOrDefault(x => x.PresetName.ToLower() == arguments.At(1).ToLower());
        if (conf is null)
        {
            response = $"Could not find preset \"{arguments.At(1)}\"";
            return false;
        }

        bool failed = false;
        foreach (PropertyInfo property in ev.GetType().GetProperties())
        {
            if (property.GetCustomAttribute<EventConfigAttribute>() is null)
                continue;
            if (property.GetCustomAttribute<EventConfigPresetAttribute>() is not null)
                continue;
            if (property.PropertyType != conf.GetType() && property.PropertyType.BaseType != conf.GetType())
                continue;
            DebugLogger.LogDebug($"[{conf.PresetName}->{property.Name}] Property: {property.PropertyType?.Name} PropBase: {property.PropertyType.BaseType?.Name}, Conf: {conf.GetType()?.Name}, ConfBase: {conf.GetType().BaseType?.Name}");
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

        if (failed)
        {
            response = "Could not load config presets due to an error.";
            return false;
        }
        
        response = $"Successfully selected preset {conf.PresetName} for event \"{ev.Name}\".";
        return true; 
    BasicUsage:
    response += $"Command Usage: \n";
    if(!IsConsoleCommandSender)
        response += $"  <color=yellow>select [event] [preset / default]</color>";
    else
        response += $"  select [event] [preset / default]";

        return false;
    }
}