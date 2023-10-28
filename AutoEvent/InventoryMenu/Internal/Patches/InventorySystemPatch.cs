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
using InventorySystem.Items;
using NorthwoodLib.Pools;
using PluginAPI.Core;

namespace InventoryMenu.Internal.Patches;

[HarmonyPatch(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory.ServerSendItems))]
public static class InventorySystemPatch
{
    public static bool Prefix(InventorySystem.Inventory __instance)
    {
        if (__instance.isLocalPlayer)
        {
            return false;
        }

        HashSet<ItemIdentifier> hashSet = HashSetPool<ItemIdentifier>.Shared.Rent();
        Player ply = Player.Get(__instance._hub);
        var instance = MenuManager.Menus.FirstOrDefault(x => x.PlayerInventoryCaches.ContainsKey(ply));
        
        foreach (KeyValuePair<ushort, ItemBase> item in instance?.PlayerInventoryCaches[ply].Items ?? __instance.UserInventory.Items)
        {
            hashSet.Add(new ItemIdentifier(item.Value.ItemTypeId, item.Key));
        }
        
        __instance.TargetRefreshItems(hashSet.ToArray());
        HashSetPool<ItemIdentifier>.Shared.Return(hashSet);
        return false;
    }
}