using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow))]
    public static class ThrowableItemPatch
    {
        public static void Postfix(ThrowableItem __instance, float forceAmount, float upwardFactor, Vector3 torque, Vector3 startVel)
        {
            Scp018Projectile proj = (Scp018Projectile)__instance.Projectile;
            DebugLogger.LogDebug($"{proj.name}");
            proj._maximumVelocity = 1000000;
            proj._lastVelocity = 1000000;
        }
    }
}
