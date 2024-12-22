using HarmonyLib;
using InventorySystem;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]
internal static class ClientDropItem
{
    public static bool Prefix()
    {
        if (AutoEvent.ActiveEvent != null)
        {
            return false;
        }

        return true;
    }
}