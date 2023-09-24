// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ExplosionGrenade.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/23/2023 2:56 PM
//    Created Date:     09/23/2023 2:56 PM
// -----------------------------------------

using AutoEvent.API;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))]
class Patch
{
    public static void Prefix(TimeGrenade __instance)
    {
        if (__instance is Scp018Projectile && Extensions.RockList.ContainsKey(__instance.Info.Serial))
        {
            RockSettings settings = Extensions.RockList[__instance.Info.Serial];
            __instance.gameObject.AddComponent<Rock>().Init(__instance.gameObject, __instance.PreviousOwner, settings.FriendlyFire, settings.ThrowDamage, settings.ExplodeOnCollision);
            return;
        }
        if (__instance is Scp018Projectile && Extensions.ExplodeOnCollisionList.ContainsKey(__instance.Info.Serial))
        {
            __instance.gameObject.AddComponent<Rock>().Init(__instance.gameObject, __instance.PreviousOwner, true, 10f, true, false, Extensions.ExplodeOnCollisionList[__instance.Info.Serial]);
            return;
        }

        if (__instance is TimeGrenade && Extensions.ExplodeOnCollisionList.ContainsKey(__instance.Info.Serial))
        {
            __instance.gameObject.AddComponent<GrenadeCollision>().Init(Extensions.ExplodeOnCollisionList[__instance.Info.Serial]);
        }
    }
}