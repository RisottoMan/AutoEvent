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
public class ImpactGrenade : ICommand, IUsageProvider, IPermission
{
    public string Command { get; } = "impact";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Gives a user an impact grenade.";
    public string[] Usage { get; } = new[] { "%player%", "[frag / flash / ball]" };
    public string Permission { get; set; } = "ev.debug";
    public bool SanitizeResponse => false;
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

            ItemType projectileType = ItemType.GrenadeHE;
            if (arguments.Count >= 2)
            {
                switch (arguments.At(1).ToLower())
                {
                    case "he" or "explosive" or "explosion" or "frag":
                        projectileType = ItemType.GrenadeHE;
                        break;
                    case "flash" or "flashbang":
                        projectileType = ItemType.GrenadeFlash;
                        break;
                    case "ball" or "scp018" or "018":
                        projectileType = ItemType.SCP018;
                        break;
                }
            }

            /*if (projectileType == ItemType.SCP018)
            {
                response = "Scp018 has not yet been implemented. :(";
                return false;
            }*/

            var item = ply.AddItem(projectileType);
            item.ExplodeOnCollision();
            DebugLogger.LogDebug($"Item Serial: {item.ItemSerial}");
            response = $"Impact grenade has been given.";
            return true;
        }

        response = "Please specify a player.";
        return false;
    }
}