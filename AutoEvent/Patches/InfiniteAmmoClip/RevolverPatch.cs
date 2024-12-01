using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;

namespace AutoEvent.Patches;

//[HarmonyPatch(typeof(CylinderAmmoModule),nameof(CylinderAmmoModule.AmmoStored), MethodType.Getter)]
public class RevolverPatch
{
    // The revolver has own module for ammo
    public static bool Postfix(CylinderAmmoModule __instance, ref int __result)
    {
        if (!Player.TryGet(__instance.Firearm.Owner, out Player ply))
        {
            return true;
        }
        
        if (!Extensions.InfiniteAmmoList.ContainsKey(ply))
        {
            return true;
        }
        
        __result = __instance.AmmoMax;
        return false;
    }
}