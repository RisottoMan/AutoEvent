// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         Extensions.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 3:17 PM
//    Created Date:     10/27/2023 3:17 PM
// -----------------------------------------

using InventoryMenu.API.Features;
using PluginAPI.Core;

namespace InventoryMenu.API;

public static class Extensions
{
    public static void ShowMenu(this Player ply, Menu menu) => menu.ShowToPlayer(ply);
    public static void HideMenu(this Player ply)
    {
        var instance = MenuManager.Menus.FirstOrDefault(x => x.CanPlayerSee(ply));
        
        if (instance is null)
            return; 
        
        instance.HideForPlayer(ply);
    }

    public static void RefreshInventory(this Player ply)
    {
        ply.ReferenceHub.inventory.SendItemsNextFrame = true;
    }
    
}