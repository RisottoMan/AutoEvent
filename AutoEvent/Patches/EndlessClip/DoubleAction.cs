using AutoEvent.API;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;

namespace AutoEvent.Patches;


[HarmonyPatch(typeof(DoubleActionModule), nameof(DoubleActionModule.ServerProcessCmd))]
public class DoubleAction
{
    [HarmonyPostfix()]
    public static void Postfix(DoubleActionModule __instance)
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
        //__instance.Firearm..Status = new FirearmStatus(__instance.Firearm..MaxAmmo, __instance.Firearm.TierFlags, __instance.Firearm.Attachments);
    }
}