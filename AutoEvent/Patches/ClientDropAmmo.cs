using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem;

namespace AutoEvent.Patches;
[HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropAmmo__Byte__UInt16))]
internal static class ClientDropAmmo
{
    public static bool Prefix()
    {
        DropAmmoArgs dropAmmoEvent = new DropAmmoArgs();
        Players.OnDropAmmo(dropAmmoEvent);

        return dropAmmoEvent.IsAllowed;
    }
}