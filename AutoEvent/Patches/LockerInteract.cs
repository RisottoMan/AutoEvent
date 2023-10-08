using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using MapGeneration.Distributors;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
    internal class LockerInteract
    {
        public static bool Prefix(Locker __instance, ReferenceHub ply, byte colliderId)
        {
            LockerInteractArgs lockerInteractEvent = new LockerInteractArgs(__instance, ply);
            Players.OnLockerInteract(lockerInteractEvent);

            return lockerInteractEvent.IsAllowed;
        }
    }
}
