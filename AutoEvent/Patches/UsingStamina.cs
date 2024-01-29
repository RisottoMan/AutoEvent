using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.StaminaUsageMultiplier), MethodType.Getter)]
    internal class UsingStamina
    {
        public static void Postfix(Inventory __instance, ref float __result)
        {
            UsingStaminaArgs usingStaminaArgs = new UsingStaminaArgs(__instance._hub, __result);
            Players.OnUsingStamina(usingStaminaArgs);
            __result *= usingStaminaArgs.IsAllowed ? 1 : 0;
        }
    }
}
