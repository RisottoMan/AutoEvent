#if !EXILED
// -----------------------------------------------------------------------
// <copyright file="SearchingPickupEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using PluginAPI.Core;

using static HarmonyLib.AccessTools;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ReceiveRequestUnsafe))]
    internal static class SearchPickUpItemPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = new List<CodeInstruction>(instructions);
            
            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SearchPickUpItemArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;

            newInstructions[index].labels.Add(retLabel);

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            var playerGetGenericMethod = typeof(Player).GetMethods().First(r => r.Name == nameof(Player.Get)
                && r.GetParameters()[0].ParameterType == typeof(ReferenceHub));

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchCoordinator), nameof(SearchCoordinator.Hub))),
                    new(OpCodes.Call, playerGetGenericMethod),

                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Target))),

                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Body))),

                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldind_Ref),
                    
                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Target))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchCoordinator), nameof(SearchCoordinator.Hub))),
                    new(OpCodes.Callvirt, Method(typeof(ItemPickupBase), nameof(ItemPickupBase.SearchTimeForPlayer))),

                    new(OpCodes.Newobj, Constructor(typeof(SearchPickUpItemArgs), new[] { typeof(Player), typeof(ItemPickupBase), typeof(SearchSession), typeof(SearchCompletor), typeof(float) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    
                    new(OpCodes.Call, Method(typeof(Players), nameof(Players.OnSearchPickUpItem))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchPickUpItemArgs), nameof(SearchPickUpItemArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchPickUpItemArgs), nameof(SearchPickUpItemArgs.SearchCompletor))),
                    new(OpCodes.Stind_Ref),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchPickUpItemArgs), nameof(SearchPickUpItemArgs.SearchSession))),
                    new(OpCodes.Stloc_1),
                });

            offset = -5;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_S && i.operand is LocalBuilder { LocalIndex: 4 }) + offset;

            newInstructions.RemoveRange(index, 5);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchPickUpItemArgs), nameof(SearchPickUpItemArgs.SearchTime))),
                });

            foreach (var instruction in newInstructions)
            {
                yield return instruction;
            }
        }
    }
}
#endif