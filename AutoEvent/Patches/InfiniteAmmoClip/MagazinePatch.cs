using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(MagazineModule),nameof(MagazineModule.AmmoStored), MethodType.Getter)]
public class MagazinePatch
{
    // All guns except the revolver have this module
    public static bool Prefix(MagazineModule __instance, ref int __result)
    {
        if (!Player.TryGet(__instance.Firearm.Owner, out Player ply))
        {
            return true;
        }
        
        if (!Extensions.InfiniteAmmoList.ContainsKey(ply))
        {
            return true;
        }

        if (!__instance.MagazineInserted)
        {
            return true;
        }
        
        __result = __instance.AmmoMax + 1;
        return false;
    }
}