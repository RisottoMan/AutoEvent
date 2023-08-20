using HarmonyLib;
using NorthwoodLib.Pools;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.InstanceMode), MethodType.Setter)]
    public class ClientInstanceMode
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label skip = generator.DefineLabel();

            newInstructions[0].labels.Add(skip);

            newInstructions.InsertRange(0, new List<CodeInstruction>()
            {
                new (OpCodes.Ldsfld, AccessTools.Field(typeof(Extensions), nameof(Extensions.Dummy))),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new (OpCodes.Callvirt, AccessTools.Method(typeof(List<ReferenceHub>), nameof(List<ReferenceHub>.Contains))),
                new (OpCodes.Brfalse_S, skip),
                new (OpCodes.Ldc_I4_2),
                new (OpCodes.Starg_S, 1),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
