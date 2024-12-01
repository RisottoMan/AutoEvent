using HarmonyLib;
using InventorySystem;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo))]
internal class ServerDropAmmo
{
    public static bool Prefix()
    {
        if (AutoEvent.ActiveEvent == null)
        {
            return true;
        }
        
        return false;
    }
}