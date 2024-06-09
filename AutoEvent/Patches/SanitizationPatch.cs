using System.Collections.Generic;
using CommandSystem;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace AutoEvent.Patches;
internal static class SanitizationPatch
{
    private static bool SanitizeResponse => false;
    
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
        
        int index = newInstructions.FindIndex(x => x.OperandIs(AccessTools.PropertyGetter(typeof(ICommand), nameof(ICommand.SanitizeResponse))));
        
        newInstructions[index].operand = AccessTools.PropertyGetter(typeof(SanitizationPatch), nameof(SanitizeResponse));
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}