// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          InventoryMenu
//    FileName:         DropItemPatch.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/27/2023 11:19 PM
//    Created Date:     10/27/2023 11:19 PM
// -----------------------------------------

using HarmonyLib;
using InventoryMenu.API;
using InventoryMenu.API.EventArgs;
using InventorySystem;
using PluginAPI.Core;
using Log = InventoryMenu.API.Log;

namespace InventoryMenu.Internal.Patches;

[HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]

internal class DropItemPatch
{
    internal static bool Prefix(Inventory __instance, ushort itemSerial, bool tryThrow)
    {
        if (tryThrow)
            return true;
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
            item.OnClicked(new MenuItemClickedArgs(ply, item, false));
        }
        catch (Exception e)
        {
            Log.Warn($"Caught an error while processing menu click.");
            Log.Debug($"MenuClick (Right) Error: \n{e}");   
        }
        return false;
    }
}