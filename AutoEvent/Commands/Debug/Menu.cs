// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Menu.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 6:33 PM
//    Created Date:     10/27/2023 6:33 PM
// -----------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using AutoEvent.Interfaces;
using CommandSystem;
using InventoryMenu.API.Features;
using InventoryMenu.API;
using InventoryMenu;
using InventoryMenu.API.EventArgs;
using PluginAPI.Core;

namespace AutoEvent.Commands.Debug;

public class Menu : ICommand, IPermission
{
    public string Permission { get; set; } = "ev.debug";
    public string Command => nameof(Menu);
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (!sender.CheckPermission(Permission, out bool isConsole))
        {
            response = "You don't have permission to use this command.";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "You must specify a player";
            return false;
        }
        var player = Player.Get(arguments.At(0));
        if (player is null)
        {
            response = $"Could not find player \"{arguments.At(0)}\"";
            return false;
        }

        InventoryMenu.API.Features.Menu menu = new InventoryMenu.API.Features.Menu("Test menu.");
        menu.AddItem(new MenuItem(ItemType.Coin, "", (MenuItemClickedArgs ev) => {}));
        player.ShowMenu(menu);
        
        response = "Menu ";
        return true;
    }

    public string[] Aliases => Array.Empty<string>();
    public string Description => "Provides a test menu to a player.";
}