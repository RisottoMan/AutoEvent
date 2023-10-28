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
    public MenuItemClickedArgs(Player player, MenuItem menuItemClicked, bool isLeftClick, bool isAllowed = true)
    {
        if (menuItemClicked is null)
        {
            Log.Warn("Menu item clicked was null. This is an error!");
            return;
        }
        Player = player;
        MenuItemClicked = menuItemClicked;
        IsLeftClick = isLeftClick;
        IsAllowed = isAllowed;
    }
    public Player Player { get; private set; }
    public MenuItem MenuItemClicked { get; private set; }
    public bool IsLeftClick { get; set; }
    public bool IsAllowed { get; set; }
}