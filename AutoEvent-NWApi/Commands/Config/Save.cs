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


public class Save : ICommand, IUsageProvider
{
    public string Command => nameof(Save);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Saves an updated config or preset for future use.";
    public string[] Usage => new string[] { "[Preset / Default]" };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {

#if EXILED
        if (!sender.CheckPermission("ev.config.save"))
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

        
        DebugLogger.Debug = true;
        response = "Debug mode has been enabled.";
        return true;
    }
}