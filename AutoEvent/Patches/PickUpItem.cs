using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using PluginAPI.Core;

namespace AutoEvent.Patches
{
    // Need rework
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickUpItem
    {
        public static void Prefix(ItemSearchCompletor __instance)
        {
            PickUpItemArgs pickUpEvent = new PickUpItemArgs(Player.Get(__instance.Hub), __instance.TargetItem, __instance.TargetPickup);
            Players.OnPickUpItem(pickUpEvent);

            if (pickUpEvent.IsAllowed)
            {
                PickupSyncInfo info = __instance.TargetPickup.Info;
                info.InUse = false;
                __instance.TargetPickup.NetworkInfo = info;
                return;
            }
        }
    }
}
