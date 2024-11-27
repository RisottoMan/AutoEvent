#if !EXILED
// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using AutoEvent.Events.EventArgs;
using AutoEvent.Events;
using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using NorthwoodLib.Pools;
using PluginAPI.Core;
using UnityEngine;
using AutoEvent.Events.Handlers;

namespace AutoEvent.Patches
{
#pragma warning disable SA1402 // File may only contain a single type

    using static HarmonyLib.AccessTools;
    
    //[HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerProcessTargetHit))]
    internal static class Shot
    {
        /// <summary>
        /// I DON'T CARE.
        /// </summary>
        /// <param name="player">Fuck Player.</param>
        /// <param name="hit">Fuck Hit.</param>
        /// <param name="destructible">FuckDestructible.</param>
        /// <param name="damage">FuckDamage.</param>
        /// <returns>FuckReturn.</returns>
        internal static bool ProcessShot(ReferenceHub player, RaycastHit hit, IDestructible destructible, ref float damage)
        {
            NewShotEventArgs shotEvent = new(Player.Get(player), hit, destructible, damage);

            global::AutoEvent.Events.Handlers.Players.OnShot(shotEvent);

            if (shotEvent.CanHurt)
                damage = shotEvent.Damage;

            return shotEvent.CanHurt;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            Label jump = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(NewShotEventArgs));

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Stloc_0) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Hub
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HitboxIdentity), nameof(HitboxIdentity.TargetHub))),

                    // this.Firearm
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SingleBulletHitscan), nameof(SingleBulletHitscan.Firearm))),

                    // hit
                    new(OpCodes.Ldarg_2),

                    // destructible
                    new(OpCodes.Ldloc_1),

                    // damage
                    new(OpCodes.Ldloca_S, 0),

                    new(OpCodes.Call, Method(typeof(Shot), nameof(ProcessShot), new[] { typeof(ReferenceHub), typeof(Firearm), typeof(RaycastHit), typeof(IDestructible), typeof(float).MakeByRefType(), })),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
#endif