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

                if (AutoEvent.Singleton.Config.IsJailbirdHasInfinityCharges)
                {
                    __instance.TotalChargesPerformed = 0;
                }


                if (AutoEvent.Singleton.Config.IsJailbirdAbilityEnable) return;
                __instance._chargeAutoengageTime = 99999;
                __instance._chargeDetectionDelay = 99999;
                __instance._chargeReadyTime = 99999;
                __instance._chargeDuration = 0;
                __instance._firstChargeFrame = true;
                __instance._chargeLoading = false;
                __instance._chargeAnyDetected = false;
                __instance._chargeResetTime = 9999f;
                __instance._chargeLoadElapsed = 0;
            }
        }
    }
}