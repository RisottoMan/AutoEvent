using AdminToys;
using HarmonyLib;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(AdminToyBase), nameof(AdminToyBase.UpdatePositionServer))]
    internal static class AdminToyPosition
    {
        public static bool Prefix(AdminToyBase __instance)
        {
            DebugLogger.LogDebug(__instance.name);
            if (__instance.name.EndsWith("-Static")) return false;
            return true;
        }
    }
}
