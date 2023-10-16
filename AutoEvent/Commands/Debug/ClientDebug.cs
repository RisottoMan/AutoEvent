// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ClientDebug.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/15/2023 6:05 PM
//    Created Date:     10/15/2023 6:05 PM
// -----------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;

namespace AutoEvent.Commands.Debug;

// Dont forget to disable this. - for debugging via f3 + f4 console.
[CommandHandler(typeof(ClientCommandHandler))]
public class ClientDebug : ICommand, IPermission
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        string id = Player.Get(sender).UserId;
        if (DebugLogger.ForwardDebugLogs.Contains(id))
        {
            DebugLogger.ForwardDebugLogs.Remove(id);
            response = "Disabled Forwarding Debug Logging";
            return true;
        }
        
        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (DebugLogger.ForwardDebugLogs.Contains(id))
        {
            DebugLogger.ForwardDebugLogs.Add(id);
        }

        response = "Enabled Forwarding Debug Logging.";
        return true;
    }

    public string Command => "evdebug";
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Toggles some basic debug information for the user.";
    public string Permission { get; set; } = "ev.debugconsole";
}