using System.Collections.Generic;
using System.Reflection.Emit;
using AutoEvent.Events.EventArgs;
using HarmonyLib;
using InventorySystem.Items.Jailbird;
using Mirror;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace AutoEvent.Patches;
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
                // this (JailbirdItem)
                new CodeInstruction(OpCodes.Ldarg_0),

                // header (JailbirdMessageType)
                new CodeInstruction(OpCodes.Ldloc_0),

                // HandleJailbird(JailbirdItem, JailbirdMessageType)
                new(OpCodes.Call, Method(typeof(JailbirdPatch), nameof(HandleJailbird))),

                // return false if not allowed
                new(OpCodes.Brfalse_S, retLabel),
            }
        );

        newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

        for (int z = 0; z < newInstructions.Count; z++)
            yield return newInstructions[z];

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    /// <summary>
    /// Processes Jailbird statuses.
    /// </summary>
    /// <param name="instance"> <see cref="JailbirdItem"/> instance. </param>
    /// <param name="messageType"> <see cref="JailbirdMessageType"/> type. </param>
    /// <returns> <see cref="bool"/>. </returns>
    private static bool HandleJailbird(JailbirdItem instance, JailbirdMessageType messageType)
    {
        if (AutoEvent.EventManager.CurrentEvent == null)
            return true;
        if (Extensions.JailbirdIsInvincible || Extensions.InvincibleJailbirds.Contains(instance))
        {
            instance.TotalChargesPerformed--;
        }
        switch (messageType)
        {
            case JailbirdMessageType.AttackTriggered:
            {
                SwingingJailbirdEventArgs ev = new(instance.Owner, instance);

                Events.Handlers.OnSwingingJailbird(ev);

                return ev.IsAllowed;
            }

            case JailbirdMessageType.ChargeStarted:
            {
                ChargingJailbirdEventArgs ev = new(instance.Owner, instance);
                
                Events.Handlers.OnChargingJailbird(ev);

                return ev.IsAllowed;
            }

            default:
                return true;
        }
    }
}


