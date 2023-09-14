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

namespace AutoEvent.Commands.Config;


public class Modify : ICommand, IUsageProvider
{
    public string Command => nameof(Modify);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Modifies an option in a config or preset for an event.";
    public string[] Usage => new string[] { "event", "preset", "option", "[value]" };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {

#if EXILED
        if (!((CommandSender)sender).CheckPermission("ev.config.modify"))
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


        if (arguments.Count < 1)
        {
            response = $"Please select an event to modify.\n" +
                       $"Usage: ";
            return false;
        }

        response = "Not Implemented Yet";
        return true;
    }
}