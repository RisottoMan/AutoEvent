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

namespace AutoEvent.Commands.Debug;
public class AntiEnd : ICommand, IUsageProvider, IPermission
{
    public string Command => nameof(AntiEnd);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Prevents an event from ending.";
    public string[] Usage => new string[] { "Enable / Disable / [Toggle]" };
    public string Permission { get; set; } = "ev.debug";
    public bool SanitizeResponse => false;
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count >= 1 && arguments.At(0).ToLower() == "enable")
        {
            DebugLogger.AntiEnd = true;
        }
        else if (arguments.Count >= 1 && arguments.At(0).ToLower() == "disable")
        {
            DebugLogger.AntiEnd = false;
        }
        else
        {
            DebugLogger.AntiEnd = !DebugLogger.AntiEnd;
        }
        response = $"Anti-End {(DebugLogger.AntiEnd ? "Enabled" : "Disabled")}.";
        return true;
    }
}