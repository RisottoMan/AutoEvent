using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Searching;
using PluginAPI.Core;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ReceiveRequestUnsafe))]
    internal static class SearchPickUpItem
    {
        public static bool Prefix(SearchCoordinator __instance)
        {
            SearchPickUpItemArgs searchPickUpEvent = new SearchPickUpItemArgs(
                Player.Get(__instance.Completor.Hub),
                __instance.Completor.TargetItem,
                __instance.Completor.TargetPickup);
            Players.OnSearchPickUpItem(searchPickUpEvent);

            return searchPickUpEvent.IsAllowed;
        }
    }
}
