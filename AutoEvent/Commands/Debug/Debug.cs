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

namespace AutoEvent.Commands.Debug;

public class Debug : ParentCommand
{
    public Debug() => LoadGeneratedCommands();
    public override void LoadGeneratedCommands()
    {
        this.RegisterCommand(new Enable());
        this.RegisterCommand(new Disable());
        this.RegisterCommand(new List());
        this.RegisterCommand(new Write());
        this.RegisterCommand(new AntiEnd());
        this.RegisterCommand(new Presets());
        this.RegisterCommand(new InfiniteAmmo());
        this.RegisterCommand(new ImpactGrenade());
        this.RegisterCommand(new Rock());
        this.RegisterCommand(new SetRole());
        this.RegisterCommand(new RNG());
        this.RegisterCommand(new PowerupCommand());
        this.RegisterCommand(new MenuCommand());
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = $"Debug mode is currently {(DebugLogger.Debug ? "On" : "Off")}. \n" +
                   "Please enter a valid subcommand: \n";
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
        /*
                   "<color=yellow>  Enable <color=white>-> Enables Debug Mode. \n" +
                   "<color=yellow>  Disable <color=white>-> Disables Debug Mode.\n" +
                   "<color=yellow>  List <color=white>-> Lists all debug options.\n" +
                   "<color=yellow>  Write <color=white>-> Writes all debug output to a log. (including past logs).\n" +
                   "<color=yellow>  AntiEnd <color=white>-> Prevents an event from ending.\n" +
                   "<color=yellow>  Presets <color=white>-> Logs the available presets for an event.\n";
        */
        return false;
    }

    public override string Command => nameof(global::AutoEvent.Commands.Debug);
    public override string[] Aliases => Array.Empty<string>();
    public override string Description => "Runs various debug functions";
}