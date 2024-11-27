using AutoEvent.API;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;

namespace AutoEvent.Patches;
[HarmonyPatch(typeof(PumpActionModule), nameof(PumpActionModule.ServerProcessShot))]
public class PumpAction
{
    [HarmonyPostfix()]
    public static void Postfix(PumpActionModule __instance)
    {
        if (__instance.Firearm is null)
        {
            return;
        }

        if (__instance.Firearm.Footprint.Hub is null)
        {
            return;
        }

        Player ply = Player.Get(__instance.Firearm.Owner);
        if (ply is null)
        {
            return;
        }
        if (Extensions.InfiniteAmmoList is null || !Extensions.InfiniteAmmoList.ContainsKey(ply) || !Extensions.InfiniteAmmoList[ply].HasFlag(AmmoMode.EndlessClip))
        {
            return;
        }
        
        //FirearmStatusFlags flags = __instance._firearm.Status.Flags;
        //flags.SetFlag(FirearmStatusFlags.Chambered, true);
        //flags.SetFlag(FirearmStatusFlags.Cocked, true);
        //__instance._firearm.Status = new FirearmStatus(__instance._firearm.AmmoManagerModule.MaxAmmo, flags, __instance._firearm.Status.Attachments);
    }
}