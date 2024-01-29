using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(Scp018Physics), nameof(Scp018Physics.ServerUpdatePrediction))]
    internal static class Scp018Bounce
    {
        public static void Prefix(Scp018Physics __instance)
        {
            Scp018BounceArgs scp018BounceArgs = new Scp018BounceArgs(__instance._scp018.PreviousOwner.Hub, __instance.Pickup, __instance._scp018);
            Servers.OnScp018Bounce(scp018BounceArgs);
        }
    }
}
