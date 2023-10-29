// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         SelectItemPatch.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 11:08 PM
//    Created Date:     10/27/2023 11:08 PM
// -----------------------------------------

using HarmonyLib;
using InventoryMenu.API;
using InventoryMenu.API.EventArgs;
using InventorySystem;
using PluginAPI.Core;
using Log = InventoryMenu.API.Log;

namespace InventoryMenu.Internal.Patches;

[HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdSelectItem__UInt16))]
internal static class SelectItemPatch
{
    internal static bool Prefix(Inventory __instance, ushort itemSerial)
    {
        Player ply = Player.Get(__instance._hub);
        var instance = MenuManager.Menus.FirstOrDefault(x => x.CanPlayerSee(ply));
        if (instance is null)
        {
            return true;
        }

        var item = instance.Items.FirstOrDefault(x => x.Value.Serial == itemSerial).Value;
        if (item is null)
        {
            Log.Debug($"Item is null. Items Avail: {instance.Items.Values.Select(x => x.Serial.ToString()).Aggregate((x, y) => x += $" {y}")}");
            return false;
        }

        try
        {
            item.OnClicked(new MenuItemClickedArgs(ply, item, true));
        }
        catch (Exception e)
        {
            Log.Warn($"Caught an error while processing menu click.");
            Log.Debug($"MenuClick (Left) Error: \n{e}");
        }
        return false;
    }
}