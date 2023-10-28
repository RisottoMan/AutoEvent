/*// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         PlayerInventory.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 3:37 PM
//    Created Date:     10/27/2023 3:37 PM
// -----------------------------------------

using HarmonyLib;
using InventorySystem.Items;
using PluginAPI.Core;
using PluginAPI.Core.Items;

namespace InventoryMenu.API.Features;

public class PlayerInventoryCache
{
    public PlayerInventoryCache(Player player)
    {
        Player = player;
        InventoryIsCleared = false;
        Items = new Dictionary<ushort, ItemBase>();
    }

    /// <summary>
    /// The player.
    /// </summary>
    public Player Player { get; private set; }
    public bool InventoryIsCleared { get; private set; }
    
    /// <summary>
    /// The players inventory.
    /// </summary>
    public Dictionary<ushort, ItemBase> Items { get; private set; }

    /// <summary>
    /// Stores and clears the inventory of the player.
    /// </summary>
    public void StoreAndClearInventory()
    {
        if (Player is null)
        {
            return;
        }

        if (Player.ReferenceHub.inventory.UserInventory.Items is null)
        {
            return;
        }

        InventoryIsCleared = true;
        // Store Values.
        Items = Player.ReferenceHub.inventory.UserInventory.Items;
        try
        {
            Dictionary<int, ushort> itemIds = new Dictionary<int, ushort>();
            foreach (var item in Player.ReferenceHub.inventory.UserInventory.Items)
            {
                itemIds.Add(itemIds.Count, item.Key);
            }

            for (int i = 0; i < Player.ReferenceHub.inventory.UserInventory.Items.Count; i++)
            {
                if (!itemIds.ContainsKey(i))
                {
                    continue;
                }

                var item = Player.ReferenceHub.inventory.UserInventory.Items.Remove(itemIds[i]);
            }
        }
        catch (Exception e)
        {
            Log.Warn($"Could not clear the users inventory. Exception: \n{e}");
        }
    }
    
    /// <summary>
    /// Restores the player's old inventory.
    /// </summary>
    public void RestoreInventory()
    { 
        /*Player.ReferenceHub.inventory.UserInventory.Items = Items;
        Items.Clear();
        InventoryIsCleared = false;
        *//*
    }


    /// <summary>
    /// Method Not Recommended.
    /// </summary>
    public void ForceClearedInventory(bool inventoryIsCleared)
    {
        InventoryIsCleared = inventoryIsCleared;
    }
}
*/