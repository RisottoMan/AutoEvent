using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using HarmonyLib;
using InventorySystem;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]
internal static class ClientDropItem
{
    public static bool Prefix()
    {
        if (AutoEvent.EventManager.CurrentEvent is Event activeEvent)
        {
            if (!activeEvent.EventHandlerSettings.HasFlag(EventFlags.IgnoreDroppingItem))
            {
                return false;
            }
        }

        return true;
    }
}