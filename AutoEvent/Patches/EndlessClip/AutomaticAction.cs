using System.Linq;
using AutoEvent.API;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(AutomaticActionModule), nameof(AutomaticActionModule.ServerProcessCmd))]
public class AutomaticAction
{
    [HarmonyPostfix()]
    public static void Postfix(AutomaticActionModule __instance)
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
        
        //FirearmStatusFlags firearmStatusFlags = __instance._firearm.Status.Flags;
        //firearmStatusFlags.SetFlag(FirearmStatusFlags.Chambered, true);
        //firearmStatusFlags.SetFlag(FirearmStatusFlags.Cocked, true);
        //__instance._firearm.Status = new FirearmStatus((byte)((int)__instance._firearm.AmmoManagerModule.MaxAmmo), firearmStatusFlags, __instance._firearm.Status.Attachments);
    }
}