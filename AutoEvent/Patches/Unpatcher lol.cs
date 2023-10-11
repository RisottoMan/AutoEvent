// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Unpatcher lol.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/26/2023 9:11 PM
//    Created Date:     09/26/2023 9:11 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace AutoEvent.Patches;
/* Ignore this - this was for confirmation bias (I was right ... )
[HarmonyPatch]
public class Unpatcher_lol
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        DebugLogger.LogDebug("Getting Method.");
        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName.ToLower().Contains("mapeditorreborn"));
        if (assembly is null)
        {
            return null;
        }
        DebugLogger.LogDebug("Found MER.");
        var types = assembly.DefinedTypes.FirstOrDefault(type => type.Name.ToLower().Contains("lightsourceupdatepatch"));
        if (types is null)
        {
            DebugLogger.LogDebug("type is null");
            return null;
        }
        var methods = types.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        if (!methods.Any())
        {
            DebugLogger.LogDebug("No methods 1.");
            return null;
        }
        var sortedMethods = methods.Where(method => method.Name == "Prefix");
        if (!sortedMethods.Any())
        {
            DebugLogger.LogDebug("No methods 2.");
            return null;
        }
        DebugLogger.LogDebug("Found Patch.");
        return methods.Cast<MethodBase>();
    }
    [HarmonyPrefix]
    public static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}*/