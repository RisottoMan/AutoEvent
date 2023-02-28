using HarmonyLib;
using InventorySystem.Items.Jailbird;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.Patch
{
    internal class JailBirdPatch
    {
        [HarmonyPatch(typeof(JailbirdItem), "ServerProcessCmd")]
        static class Update
        {
            internal static void Prefix(JailbirdItem __instance, NetworkReader reader)
            {
                if (AutoEvent.ActiveEvent == null) return;
                __instance._chargeDuration = 0;
            }
        }
    }
}
