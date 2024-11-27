// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GetPlayerAmmo.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 4:02 PM
//    Created Date:     09/19/2023 4:02 PM
// -----------------------------------------

using AutoEvent.API;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;

namespace AutoEvent.Patches;

//[HarmonyPatch(typeof(InventorySystem.Items.Firearms.Modules.AutomaticAmmoManager),nameof(InventorySystem.Items.Firearms.Modules.AutomaticAmmoManager.UserAmmo),MethodType.Getter)]
public class GetPlayerAmmo
{
    public static bool Prefix(AutomaticActionModule __instance, ref ushort __result) //AutomaticAmmoManager __instance, ref ushort __result
    {
        Player ply = Player.Get(__instance.Firearm.Owner);
        if (ply is null)
        {
            return true;
        }
        
        if (Extensions.InfiniteAmmoList is null || !Extensions.InfiniteAmmoList.ContainsKey(ply))
        {
            return true;
        }
        
        //__instance.Firearm.Owner.inventory.ServerSetAmmo(__instance.Firearm.AmmoType,  __instance.Firearm.AmmoManagerModule.MaxAmmo);
        //__result = __instance.Firearm.AmmoManagerModule.MaxAmmo;
        return false;
    }
}