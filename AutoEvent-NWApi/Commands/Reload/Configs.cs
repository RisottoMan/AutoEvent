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
using Exiled.API.Features;

namespace AutoEvent.Commands.Reload;


public class Configs : ICommand
{
    public string Command => nameof(Configs);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Reloads all events";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {

#if EXILED
        if (!((CommandSender)sender).CheckPermission("ev.reload"))
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

        foreach (var ev in Event.Events)
        {
            ev.LoadConfigs();
        }
        response = $"Reloaded Configs and Config Presets.";

        response = $"Reloaded Events. {Event.Events.Count} events have been found.";
        return true;
    }
}