// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         MenuInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 3:18 PM
//    Created Date:     10/27/2023 3:18 PM
// -----------------------------------------

namespace InventoryMenu.API.Features;

/// <summary>
/// Contains info about a menu.
/// </summary>
public struct MenuInfo
{
    /// <summary>
    /// The amount of items a menu has.
    /// </summary>
    public int TotalItems;
    
    /// <summary>
    /// The amount of lines a menu takes.
    /// </summary>
    public int BroadcastLines;
    public MenuInfo(int totalItems, int broadcastLines)
    {
        this.TotalItems = totalItems;
        this.BroadcastLines = broadcastLines;
    }
}