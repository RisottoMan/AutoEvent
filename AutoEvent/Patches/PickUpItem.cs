using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Searching;
using PluginAPI.Core;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.ValidateStart))]
    internal static class PickUpItem
    {
        public static bool Prefix(ItemSearchCompletor __instance)
        {
            PickUpItemArgs pickUpEvent = new PickUpItemArgs(Player.Get(__instance.Hub), __instance.TargetItem, __instance.TargetPickup);
            Players.OnPickUpItem(pickUpEvent);

            return pickUpEvent.IsAllowed;
        }
    }
}
