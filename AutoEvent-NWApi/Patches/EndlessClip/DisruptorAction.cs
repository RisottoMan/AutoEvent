// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         AmmoGetPatch.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 3:22 PM
//    Created Date:     09/19/2023 3:22 PM
// -----------------------------------------

using AutoEvent.API;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(InventorySystem.Items.Firearms.Modules.DisruptorAction),
    nameof(InventorySystem.Items.Firearms.Modules.DisruptorAction.ServerAuthorizeShot),
    MethodType.Getter)]
public class DisruptorAction
{
    [HarmonyPostfix()]
    public static void Postfix(InventorySystem.Items.Firearms.Modules.DisruptorAction __instance)
    {
        if (__instance._firearm is null)
        {
            return;
        }

        if (__instance._firearm.Footprint.Hub is null)
        {
            return;
        }

        var component = __instance._firearm.Footprint.Hub.gameObject.GetComponent<InfiniteAmmoComponent>();
        if (component is null)
        {
            return;
        }

        if (!component.EndlessClip)
        {
            return;
        }
        __instance._firearm.Status = new FirearmStatus(__instance._firearm.AmmoManagerModule.MaxAmmo, __instance._firearm.Status.Flags, __instance._firearm.Status.Attachments);
    }
}