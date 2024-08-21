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
using System.Linq;
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#endif
namespace AutoEvent.Commands.Debug;


public class Presets : ICommand, IPermission
{
    public string Command => nameof(Presets);
    public string Description => "Logs the available presets for an event.";
    public string[] Aliases => new string[] { };
    public string Permission { get; set; } = "ev.debug";
    public bool SanitizeResponse => false;
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {

        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "You must specify an event first.";
            return false;
        }

        var ev = Event.GetEvent(arguments.At(0));
        if (ev is null)
        {
            response = $"Could not find Event \"{arguments.At(0)}\"";
            return false;
        }
        string x = $"{ev.ConfigPresets.Count} Presets Available: \n";
        foreach (var preset in ev.ConfigPresets)
        {
            if(!IsConsoleCommandSender)
                x += $"<color=yellow>[{((EventConfig)preset).PresetName}]<color=white>, \n";
            else
                x += $"[{((EventConfig)preset).PresetName}], \n";
        }
        response = x;
        return true;
    }
}