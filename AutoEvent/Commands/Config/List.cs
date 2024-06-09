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
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands.Config;
public class List : ICommand, IUsageProvider, IPermission
{
    public string Command => nameof(List);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Lists available presets for an event.";
    public string[] Usage => new string[] { "event" };
    public string Permission { get; set; } = "ev.config.list";
    public bool SanitizeResponse => false;
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count == 0)
        {
            response = "Please specify an event.";
            return false;
        }

        Event ev = Event.GetEvent(arguments.At(0));
        if (ev is null)
        {
            DebugLogger.LogDebug($"Event is null.", LogLevel.Debug);
            response = "An error has occured.";
            return false;
        }

        if (ev.ConfigPresets is null)
        {
            DebugLogger.LogDebug($"Config Presets List is null.", LogLevel.Debug);
            response = "An error has occured.";
            return false;
        }

        string presets = $"Available Config Presets for Event \"{ev.Name}\": \n";
        foreach (var x in ev.ConfigPresets)
        {
            presets += $"  {((EventConfig)x).PresetName}, \n";
        }

        response = presets;
        return true;
        
    }
}