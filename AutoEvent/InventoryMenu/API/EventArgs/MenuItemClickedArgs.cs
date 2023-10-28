// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         MenuItemClickedArgs.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 2:12 PM
//    Created Date:     10/27/2023 2:12 PM
// -----------------------------------------

using InventoryMenu.API.Features;
using PluginAPI.Core;

namespace InventoryMenu.API.EventArgs;

public class MenuItemClickedArgs
{
    public MenuItemClickedArgs(Player player, MenuItem itemClicked, bool isAllowed = true)
    {
        Player = player;
        ItemClicked = itemClicked;
        IsAllowed = isAllowed;
    }
    public Player Player { get; private set; }
    public MenuItem ItemClicked { get; private set; }
    public bool IsAllowed { get; set; }
}