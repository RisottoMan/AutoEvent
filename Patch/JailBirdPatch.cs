using Exiled.API.Features;
using HarmonyLib;
using InventorySystem.Items.Jailbird;
using Mirror;

namespace AutoEvent.Patch
{
    internal class JailBirdPatch
    {
        //[HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))] // НУжны люди для проверки
        static class ServerProcessCmdPatch
        {
            internal static bool Prefix(JailbirdItem __instance, NetworkReader reader)
            {
                Log.Info("JailBird Patch");
                if (AutoEvent.ActiveEvent == null) return true;

                if (!AutoEvent.Singleton.Config.IsJailbirdAbilityEnable)
                {
                    JailbirdMessageType jailbirdMessageType = (JailbirdMessageType)reader.ReadByte();

                    switch (jailbirdMessageType)
                    {
                        case JailbirdMessageType.ChargeStarted:
                        case JailbirdMessageType.ChargeLoadTriggered:
                        case JailbirdMessageType.ChargeFailed:
                            return false;
                        default:
                            return true;
                    }
                }
                if (AutoEvent.Singleton.Config.IsJailbirdHasInfinityCharges)
                {
                    __instance.TotalChargesPerformed = 0;
                }
                return true;
            }
        }
    }
}