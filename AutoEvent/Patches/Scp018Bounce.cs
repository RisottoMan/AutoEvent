using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PluginAPI.Core;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Scp018Physics), nameof(Scp018Physics.ServerUpdatePrediction))]
    internal static class Scp018Bounce
    {
        public static void Prefix(Scp018Physics __instance)
        {
            bool isBounced = false; 
            if (!__instance._outOfBounds && __instance._lastTime - 0.20000000298023224 < NetworkTime.time)
                isBounced = true;

            Player player = Player.Get(__instance._scp018.PreviousOwner.Hub);

            Scp018BounceArgs scp018BounceArgs = new Scp018BounceArgs(player, __instance.Pickup, __instance._scp018, isBounced);
            Servers.OnScp018Bounce(scp018BounceArgs);
        }
    }
}
