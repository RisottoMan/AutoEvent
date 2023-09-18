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


public class Presets : ICommand, IUsageProvider
{
    public string Command => nameof(Presets);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Logs the available presets for an event.";
    public string[] Usage => new string[] { };
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {

#if EXILED
        if (!((CommandSender)sender).CheckPermission("ev.debug"))
        {
            response = "You do not have permission to use this command";
            return false;
        }
#else
        var config = AutoEvent.Singleton.Config;
        var player = Player.Get(sender);
        if (sender is ServerConsoleSender || sender is CommandSender cmdSender && cmdSender.FullPermissions)
        {
            goto skipPermissionCheck;
        }

        if (!config.PermissionList.Contains(ServerStatic.PermissionsHandler._members[player.UserId]))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }
#endif

        skipPermissionCheck:

        if (arguments.Count <= 1)
        {
            response = "You must specify an event first.";
            return false;
        }

        var ev = Event.Events.FirstOrDefault(x => x.Name.ToLower() == arguments.At(1).ToLower());
        if (ev is null)
        {
            response = $"Could not find Event \"{arguments.At(1)}\"";
            return false;
        }
        string x = $"{ev.ConfigPresets.Count} Presets Available: \n";
        foreach (var preset in ev.ConfigPresets)
        {
            x += $"<color=yellow>[{preset.PresetName}]<color=white>, \n";
        }
        response = x;
        return true;
    }
}