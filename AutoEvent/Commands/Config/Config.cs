// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ReloadEventsCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/03/2023 7:44 PM
//    Created Date:     09/03/2023 7:44 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands.Config;

public class Config : ParentCommand
{
    public Config() => LoadGeneratedCommands();
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new List());
        RegisterCommand(new Select());
        if (DebugLogger.Debug)
        {
            DebugLogger.LogDebug("Registering 'ev config modify' and 'ev config save' commands. They may be broken or lack functionality.", LogLevel.Warn, true);
            RegisterCommand(new Modify());
            RegisterCommand(new Save());
        }
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = "Please enter a valid subcommand: \n";
        foreach (var x in this.Commands)
        {
            string args = "";
            if (x.Value is IUsageProvider usage)
            {
                foreach (string arg in usage.Usage)
                {
                    args += $"[{arg}] ";
                }
            }
            if(sender is not ServerConsoleSender)
                response += $"<color=yellow> {x.Key} {args} <color=white>-> {x.Value.Description}. \n";
            else
                response += $" {x.Key} {args} -> {x.Value.Description}. \n";
        }
        /*"<color=yellow>  List <color=white>-> List the presets available for an event.\n" +
                   "<color=yellow>  Select <color=white>-> Select a preset to use for an event round.\n" +
                   "<color=yellow>  Modify <color=white>-> Modify a preset or config for an event.\n" +
                   "<color=yellow>  Save <color=white>-> Save an modified preset or config for future use.\n";
                   */
        return false;
    }

    public override string Command => nameof(Config);
    public override string[] Aliases => Array.Empty<string>();
    public override string Description => "Allows modifying configs before and during events";
}

/*
 * Config
 *   -> List [Event]
 *   -> Select [Event] [Preset]
 *   -> Modify [Event] [Preset] [Option] [Value]
 *   -> Save [Event] [Preset]
 */