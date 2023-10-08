using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]
    internal static class DropItem
    {
        public static bool Prefix()
        {
            DropItemArgs dropItemEvent = new DropItemArgs();
            Players.OnDropItem(dropItemEvent);

            return dropItemEvent.IsAllowed;
        }
    }
}
