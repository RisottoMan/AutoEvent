// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         GetMenuItemsForPlayerArgs.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 3:16 PM
//    Created Date:     10/28/2023 3:16 PM
// -----------------------------------------

using InventoryMenu.API.Features;
using InventorySystem.Items;
using PluginAPI.Core;

namespace InventoryMenu.API.EventArgs;

public class GetMenuItemsForPlayerArgs
{
    public GetMenuItemsForPlayerArgs(Player ply, Menu menu, Dictionary<ushort, ItemBase>? items = null)
    {
        Player = ply;
        Menu = menu;
        if (items is null || items.IsEmpty())
        {
            Items = Menu._itemBases;
        }
    }
    public Player Player { get; private set; }
    public Menu Menu { get; private set; }
    public Dictionary<ushort, ItemBase> Items { get; set; }
}