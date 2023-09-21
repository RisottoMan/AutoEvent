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
/* todo fix patches :)
[HarmonyPatch(typeof(InventorySystem.Items.Firearms.Modules.PumpAction),
    nameof(InventorySystem.Items.Firearms.Modules.PumpAction.ServerAuthorizeShot),
    MethodType.Getter)]
public class PumpAction
{
    [HarmonyPostfix()]
    public static void Postfix(InventorySystem.Items.Firearms.Modules.PumpAction __instance)
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

        FirearmStatusFlags flags = __instance._firearm.Status.Flags;
        flags.SetFlag(FirearmStatusFlags.Chambered, true);
        flags.SetFlag(FirearmStatusFlags.Cocked, true);
        __instance._firearm.Status = new FirearmStatus(__instance._firearm.AmmoManagerModule.MaxAmmo, flags, __instance._firearm.Status.Attachments);
    }
}*/