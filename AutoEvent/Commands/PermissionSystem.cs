// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         CheckPermission.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/25/2023 1:46 PM
//    Created Date:     09/25/2023 1:46 PM
// -----------------------------------------

using System;
using System.Linq;
using System.Reflection;
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
using RemoteAdmin;
#if EXILED
#endif

namespace AutoEvent.Commands;

public static class PermissionSystem
{
    private static bool _loaded = false;
    public static bool UsingNwapiPerms = false;
    private static Assembly _nwapiPermissionSystem;
    private static Type _permissionsAPIType;
    private static MethodInfo _getPermissionsMethod;

    public static bool UsingExiledPerms = false;
    private static Assembly _exiledPermissions;
    private static Type _exiledPermissionsAPIType;
    private static MethodInfo _exiledGetPermissionsMethod;
    public static void Load()
    {
        try
        {
            loadNWAPIPerms();
            loadExiledPerms();
            if (UsingExiledPerms && UsingNwapiPerms)
            {
                DebugLogger.LogDebug(
                    $"Permissions System has found both ExiledPerms and NwAPI perms. Both will be used for permissions.");
            }
            else if (UsingExiledPerms)
            {
                DebugLogger.LogDebug(
                    $"Permissions System has only found ExiledPerms. Exiled Perms will be used for permissions.");
            }
            else
            {
                DebugLogger.LogDebug(
                    $"Permissions System has only found NwAPI perms. NwAPI perms will be used for permissions.");
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Could not load NWAPIPerms or ExiledPerms. Exception: \n{e}");
        }

        _loaded = true;
    }

    private static void loadExiledPerms()
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(s => s.GetName().Name == "Exiled.Permissions");
        if (assembly == null)
        {
            DebugLogger.LogDebug(
                $"Couldn't find Exiled Permissions. Exiled will not be used for permissions even if it is installed.");
            return;
        }

        _exiledPermissions = assembly;

        var type = _exiledPermissions.DefinedTypes.FirstOrDefault(s =>
            s.FullName == "Exiled.Permissions.Extensions.Permissions");
        if (type is null)
        {
            DebugLogger.LogDebug(
                $"Couldn't find Permissions Type. You are probably using a different version of Exiled Permissions than this plugin was designed for.");
            return;
        }

        _exiledPermissionsAPIType = type;
        //CheckPermission(this CommandSender sender, string permission)
        var method =
            _exiledPermissionsAPIType.GetMethod("CheckPermission",
                new Type[] { typeof(ICommandSender), typeof(string) });
        if (method == null)
        {
            DebugLogger.LogDebug(
                $"Couldn't find the CheckPermission Method. You are probably using a different version of Exiled Permissions than this plugin was designed for.");
            return;
        }

        _exiledGetPermissionsMethod = method;
        DebugLogger.LogDebug($"Found Exiled Permissions Succesfully.");
        UsingExiledPerms = true;
    }

    private static void loadNWAPIPerms()
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(s => s.GetName().Name == "NWAPIPermissionSystem");
        if (assembly == null)
        {
            DebugLogger.LogDebug(
                $"Couldn't find PermissionHandler. You are probably using a different version of NWAPIPermissionsSystem than this plugin was designed for.");
            return;
        }

        _nwapiPermissionSystem = assembly;

        var type = _nwapiPermissionSystem.DefinedTypes.FirstOrDefault(s => s.Name == "PermissionHandler");
        if (type is null)
        {
            DebugLogger.LogDebug(
                $"Couldn't find PermissionHandler. You are probably using a different version of NWAPIPermissionsSystem than this plugin was designed for.");
            return;
        }

        _permissionsAPIType = type;
        //CheckPermission(this CommandSender sender, string permission)
        var method =
            _permissionsAPIType.GetMethod("CheckPermission", new Type[] { typeof(ICommandSender), typeof(string) });
        if (method == null)
        {
            DebugLogger.LogDebug(
                $"Couldn't find the CheckPermission Method. You are probably using a different version of NWAPIPermissionsSystem than this plugin was designed for.");
            return;
        }

        _getPermissionsMethod = method;
        DebugLogger.LogDebug($"Found NWAPIPermissionsHandler succesfully.");
        UsingNwapiPerms = true;
    }

    public static bool CheckPermission(this Player ply, string permissions, out bool isConsole) =>
        CheckPermission(new PlayerCommandSender(ply.ReferenceHub), permissions, out isConsole);

    public static bool CheckPermission(this CommandSender sender, string permission, out bool isConsole) =>
        CheckPermission((ICommandSender)sender, permission, out isConsole);

    public static bool CheckPermission(this ICommandSender sender, string permission, out bool isConsole)
    {
        isConsole = false;
        bool result = false;
        if (!_loaded)
        {
            Load();
        }

        // Check Sender is null
        if (sender is null)
        {
            DebugLogger.LogDebug($"Sender is null");
            return false;
        }

        // Check Sender is ServerConsole
        if (sender is ServerConsoleSender)
        {
            isConsole = true;
            return true;
        }
        // DebugLogger.LogDebug("Checking NWApi Perms.");
        // Check NWApi Perms
        if (UsingNwapiPerms)
        {
            try
            {
                result = (bool)_getPermissionsMethod.Invoke(null, new object[] { sender, permission });
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Could not get Permissions from NWAPI. Exception:\n{e}");
            }
        }

        if (result)
            return true;
        //DebugLogger.LogDebug("Checking Exiled Perms.");

        // Check Exiled Perms
        if (UsingExiledPerms)
        {
            try
            {
                result = (bool)_exiledGetPermissionsMethod.Invoke(null, new object[] { sender, permission });
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Could not get Permissions from Exiled Permissions. Exception:\n{e}");
            }
        }

        if (result)
            return true;
        //DebugLogger.LogDebug("Checking Default Perms.");
        // Check Perms Via Config
        try
        {
            var config = AutoEvent.Singleton.Config;
            if (sender is CommandSender cmdSender && cmdSender.FullPermissions)
            {
                result = true;
            }

#if !EXILED
            var player = Player.Get(sender);
            if (player is null ||
                !config.PermissionList.Contains(ServerStatic.PermissionsHandler._members[player.UserId]))
            {
                result = false;
            }
#endif
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("An error has occured while checking permissions.", LogLevel.Error, true);
            DebugLogger.LogDebug(e.ToString());
        }

        return result;
    }

}
