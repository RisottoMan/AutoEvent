// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GetPlayerPatches.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/21/2023 3:12 PM
//    Created Date:     09/21/2023 3:12 PM
// -----------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PluginAPI.Core;

namespace AutoEvent.Patches;

[HarmonyPatch()]
public class GetPlayersNWApi
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(PluginAPI.Core.Player).GetMethods()
            .Where(method => !method.IsGenericMethod && method.Name == nameof(Player.GetPlayers))
            .Cast<MethodBase>();
    }
    [HarmonyPrefix]
    public static bool Prefix(ref List<Player> __result)
    {
        var stack = new StackTrace();
        var frames = stack.GetFrames();
        if (frames is null)
        {
            return true;
        }
        for (var i = 0; i < frames.Length; i++)
        {
            var frame = frames[i];
            if (i > 8)
                break;
            var type = frame.GetMethod().DeclaringType;
            var nameSpace = type?.Namespace;
            if (!string.IsNullOrWhiteSpace(nameSpace) && nameSpace.StartsWith("AutoEvent") &&
                !nameSpace.Contains("Patches"))
            {
                if (AutoEvent.Singleton.Config.IgnoredRoles is null ||
                    AutoEvent.Singleton.Config.IgnoredRoles.Count == 0)
                    return true;
                __result = Player.GetPlayers<Player>().Where(x => !AutoEvent.Singleton.Config.IgnoredRoles.Contains(x.Role)).ToList();
                return false;
            }
            // DebugLogger.LogDebug($"trace: {frame.GetMethod().DeclaringType?.Namespace} {frame.GetMethod().DeclaringType?.Name}.{frame.GetMethod().Name}");
        }

        return true;
        
    }
}
