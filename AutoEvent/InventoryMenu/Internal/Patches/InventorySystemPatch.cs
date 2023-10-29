// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         InventorySystemPatch.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 5:21 PM
//    Created Date:     10/27/2023 5:21 PM
// -----------------------------------------

using System.Diagnostics;
using System.Text;
using HarmonyLib;
using InventoryMenu.API;
using InventoryMenu.API.EventArgs;
using InventoryMenu.API.Features;
using InventorySystem.Items;
using NorthwoodLib.Pools;
using PluginAPI.Core;
using Log = InventoryMenu.API.Log;

namespace InventoryMenu.Internal.Patches;

[HarmonyPatch(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory.ServerSendItems))]
internal static class InventorySystemPatch
{
    internal static bool Prefix(InventorySystem.Inventory __instance)
    {
        if (__instance.isLocalPlayer)
        {
            return false;
        }

        HashSet<ItemIdentifier> hashSet = HashSetPool<ItemIdentifier>.Shared.Rent();
        Player ply = Player.Get(__instance._hub);
        var instance = MenuManager.Menus.FirstOrDefault(x => x.CanPlayerSee(ply));
        GetMenuItemsForPlayerArgs? args = null;

        if (instance is not null)
        {
            try
            {
                args = new GetMenuItemsForPlayerArgs(ply, instance);
                instance.OnGetMenuItems(args);
            }
            catch (Exception e)
            {
                Log.Debug($"An error has occured while getting menu items.\n {e}");
            }
        }
        var items = (args is not null ? args.Items : __instance.UserInventory.Items);
        // Log.Debug($"[{instance is null}], Menus: {MenuManager.Menus.Count}, Total Caches: {MenuManager.Menus.Sum(x => x.PlayerInventoryCaches.Count())}");
        
        // Log.Debug($"Item Count: {items.Count}, Inv Count: {__instance.UserInventory.Items.Count} Menu null: {instance is null}, ");
        foreach (KeyValuePair<ushort, ItemBase> item in items.OrderByDescending(x => x.Key))
        {
            hashSet.Add(new ItemIdentifier(item.Value.ItemTypeId, item.Key));
        }
        
        __instance.TargetRefreshItems(hashSet.ToArray());
        HashSetPool<ItemIdentifier>.Shared.Return(hashSet);
        return false;
    }
}