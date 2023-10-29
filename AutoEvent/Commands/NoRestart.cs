// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         NoRestart.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 12:53 PM
//    Created Date:     10/28/2023 12:53 PM
// -----------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using AutoEvent.Interfaces;
using CommandSystem;
using Mirror;

namespace AutoEvent.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class NoRestart : ICommand, IPermission
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission(Permission, out bool isConsole))
        {
            response = "You do not have permission to run this command!";
            return false;
        }

        if (!AutoEvent.Singleton.Config.RestartAfterRoundFinish)
        {
            response =
                "Auto-Restart is not enabled on this server. To use this command, enable it in the AutoEvent Config.";
            return false;
        }
        
        string boldstart = isConsole ? "" : "<b>";
        string boldend = isConsole ? "" : "</b>";
        
        bool enabled = DebugLogger.NoRestartEnabled;
        if (arguments.Count >= 1)
        {
            if(arguments.At(0).ToLower() is "enable" or "enabled" or "true" or "1")
                goto enable;
            if(arguments.At(0).ToLower() is "disable" or "disabled" or "false" or "0")
                goto disable;
        }

        if (enabled)
            goto disable;
        
        goto enable;
        
        enable:
        DebugLogger.NoRestartEnabled = true;
        response =
            $"No-Restart has been enabled. The server will {boldstart}not{boldend} restart after the next event.";
        return true;
        disable:
        DebugLogger.NoRestartEnabled = false;
        response = $"No-Restart has been disabled. The server {boldstart}will{boldend} restart after the next event.";
        return true;
    }

    public string Command => nameof(NoRestart);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Disables auto-restarting the server after events are done.";
    public string Permission { get; set; } = "ev.norestart";
}