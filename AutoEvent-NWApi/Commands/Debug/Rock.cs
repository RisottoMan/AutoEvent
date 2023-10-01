// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         InfiniteAmmo.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/23/2023 11:44 AM
//    Created Date:     09/23/2023 11:44 AM
// -----------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoEvent.API;
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
using Utils.NonAllocLINQ;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands.Debug;

public class Rock : ICommand, IUsageProvider, IPermission
{
    public string Permission { get; set; } = "ev.debug";
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {

        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count >= 1)
        {
            Player ply = Player.GetPlayers().FirstOrDefault(pl => pl.Nickname.ToLower().Contains(arguments.At(0).ToLower()));
            if (ply is null)
            {
                response = $"Player \"{arguments.At(0)}\" not found. Please specify a valid player.";
                return false;
            }

            bool giveBackToOwner = arguments.Count >= 2 && arguments.At(1).ToLower() == "true";
            bool dropRock = arguments.Count >= 3 && arguments.At(2).ToLower() == "true";
            bool explodeRock = arguments.Count >= 4 && arguments.At(3).ToLower() == "true";
            int layerMask = -1;
            if (arguments.Count >= 5)
                int.TryParse(arguments.At(4), out layerMask);

            var item = ply.AddItem(ItemType.SCP018);
            item.MakeRock(new RockSettings(true, 10f, explodeRock,dropRock, giveBackToOwner){ LayerMask = layerMask});
            DebugLogger.LogDebug($"Item Serial: {item.ItemSerial}");
            foreach (var x in ply.GameObject.GetComponents(typeof(Component)))
            {
                DebugLogger.LogDebug($"{x.GetType().Name} - {x.name}");
            }
            response = $"Rock has been given.";
            return true;
        }

        response = "Please specify a player.";
        return false;
    }

    public string Command { get; } = "rock";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Gives a user a rock.";
    public string[] Usage { get; } = new[] { "%player%" };
}