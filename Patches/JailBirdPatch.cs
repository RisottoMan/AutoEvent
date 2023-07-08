using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.Events.EventArgs.Item;
using HarmonyLib;
using InventorySystem.Items.Jailbird;
using Mirror;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace AutoEvent.Patches
{
    internal class JailBirdPatch
    {
        [HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
        internal static class JailbirdPatch
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

                Label retLabel = generator.DefineLabel();

                const int offset = 2;
                int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(NetworkReader), nameof(NetworkReader.ReadByte)))) + offset;

                newInstructions.InsertRange(
                    index,
                    new[]
                    {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(JailbirdPatch), nameof(HandleJailbird))),
                    new(OpCodes.Brfalse_S, retLabel),
                    });

                newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

                for (int z = 0; z < newInstructions.Count; z++)
                    yield return newInstructions[z];

                ListPool<CodeInstruction>.Shared.Return(newInstructions);
            }
            private static bool HandleJailbird(JailbirdItem instance, JailbirdMessageType messageType)
            {
                if (AutoEvent.ActiveEvent == null) return true;

                switch (messageType)
                {
                    case JailbirdMessageType.ChargeLoadTriggered:
                        return AutoEvent.Singleton.Config.IsJailbirdAbilityEnable;
                    default:
                        return true;
                }
            }
        }
    }
}