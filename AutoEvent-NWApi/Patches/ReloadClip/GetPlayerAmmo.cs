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

using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;

namespace AutoEvent.Patches;
[HarmonyPatch(typeof(InventorySystem.Items.Firearms.Modules.AutomaticAmmoManager),nameof(InventorySystem.Items.Firearms.Modules.AutomaticAmmoManager.UserAmmo),MethodType.Getter)]
public class GetPlayerAmmo
{
    //  public static bool Prefix(AutomaticAmmoManager __instance, )
}