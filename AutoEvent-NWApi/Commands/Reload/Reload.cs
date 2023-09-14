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

namespace AutoEvent.Commands.Reload;

public class Reload : ParentCommand, IUsageProvider
{
    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new Events());
        RegisterCommand(new Configs());
        RegisterCommand(new Translations());
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = "Please enter a valid subcommand: \n" +
                   "<color=yellow>  Events <color=white>-> reload events, and external events.\n" +
                   "<color=yellow>  Configs <color=white>-> reload configs and config presets.\n" +
                   "<color=yellow>  Translations <color=white>-> reload translations.\n";
        return false;
    }

    public override string Command => nameof(Reload);
    public override string[] Aliases => Array.Empty<string>();
    public override string Description => "Reloads different aspects of the plugin and events.";
    public string[] Usage => new string[] { "[option]" };
}