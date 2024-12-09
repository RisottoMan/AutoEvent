using System.Reflection;
using HarmonyLib;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(PluginAPI.Core.Log), nameof(PluginAPI.Core.Log.Info))]
    internal static class AudioLogPatch
    {
        public static bool Prefix()
        {
            if (Assembly.GetCallingAssembly().GetName().Name == "SCPSLAudioApi")
            {
                return false;
            }

            return true;
        }
    }
}
