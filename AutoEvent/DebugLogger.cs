// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DebugLogger.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/05/2023 7:30 PM
//    Created Date:     09/05/2023 7:30 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MEC;
using PluginAPI;
using PluginAPI.Core;
using PluginAPI.Helpers;
using PluginAPI.Loader;
using UnityEngine;
using Version = GameCore.Version;

namespace AutoEvent;

public class DebugLogger
{
    static DebugLogger() 
    {
        Assemblies = new List<AssemblyInfo>();
    }
    public static DebugLogger Singleton;
    internal static List<AssemblyInfo> Assemblies { get; set; }
    internal static string SLVersion => GameCore.Version.VersionString;
    public static bool NoRestartEnabled { get; set; } = false;
    public const string Version = "9.2.2";
    public DebugLogger(bool writeDirectly)
    {
        Singleton = this;
        WriteDirectly = writeDirectly;
        _debugLogs = new List<string>();
        _loaded = true;

        if (!Directory.Exists(AutoEvent.BaseConfigPath))
        {
            Directory.CreateDirectory(AutoEvent.BaseConfigPath);
        }

        try
        {

            _filePath = Path.Combine(AutoEvent.BaseConfigPath, "debug-output.log");
            if (WriteDirectly)
            {
                DebugLogger.LogDebug($"Writing debug output directly to \"{_filePath}\"");
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }

                File.Create(_filePath).Close();
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"An error has occured while trying to create a debug log.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
    }
    
    private static string _getNWApiPlugins()
    {
        string text = "";
        try
        {

            foreach (var keyValuePair in PluginAPI.Loader.AssemblyLoader.Plugins)
            {
                try
                {
                    var version = keyValuePair.Key.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion ?? "";
                    AssemblyInfo info = new AssemblyInfo()
                    {
                        Name = keyValuePair.Key.GetName().ToString(),
                        Hash = keyValuePair.Key.ManifestModule.ModuleVersionId.ToString(),
                        Dependency = false,
                        Exiled = false,
                        Version = version
                    };
                    var hashId = keyValuePair.Key.ManifestModule.ModuleVersionId;
                    text += $"  {keyValuePair.Key.GetName()}";
                    text += $"    - Hash: {hashId}\n";
                    foreach (var plugin in keyValuePair.Value)
                    {
                        info.Plugins.Add(new PluginInfo()
                        {
                            Name = plugin.Value.PluginName, ExiledPlugin = false, Authors = plugin.Value.PluginAuthor,
                            Version = plugin.Value.PluginVersion
                        });
                        text += $"    - Plugins:\n";
                        text += $"      - {plugin.Value.PluginName} by {plugin.Value.PluginAuthor} (v{plugin.Value.PluginVersion})\n";
                    }

                    Assemblies.Add(info);
                }
                catch
                {
                }
            }

            text += "NWApi Dependencies Loaded:\n";
            foreach (var dependency in PluginAPI.Loader.AssemblyLoader.Dependencies)
            {
                try
                {
                    if (dependency is null)
                    {
                        continue;
                    }
                    var version = dependency.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion ?? "";

                    Assemblies.Add(new AssemblyInfo()
                    {
                        Name = dependency.GetName().ToString(),
                        Hash = dependency.ManifestModule.ModuleVersionId.ToString(),
                        Dependency = true,
                        Exiled = false,
                        Version = version
                    });
                    text += $"  {dependency.GetName()}\n";
                    var hashId = dependency.ManifestModule.ModuleVersionId;
                    text += $"    - Hash: {hashId}\n";
                }
                catch
                {
                }
            }

            return text;
        }
        catch (Exception)
        {
            return "";
        }

        return "";
    }
    private static string _getExiledPlugins()
    {
        try
        {
            string text = "";
            var plugins = Exiled.Loader.Loader.Plugins;
            foreach (var plugin in plugins)
            {
                try
                {
                    var version = plugin.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion ?? "";

                    AssemblyInfo info = new AssemblyInfo()
                    {
                        Name = plugin.Assembly.GetName().ToString(),
                        Hash = plugin.Assembly.ManifestModule.ModuleVersionId.ToString(),
                        Dependency = false,
                        Exiled = true,
                        Version = version,
                    };
                    info.Plugins.Add(new PluginInfo()
                    {
                        ExiledPlugin = true,
                        Authors = plugin.Author,
                        Name = plugin.Name,
                        Version = plugin.Version.ToString(),
                        Descriptions = "",
                    });
                    Assemblies.Add(info);
                    text += $"  {plugin.Assembly.GetName()}\n";
                    var hashId = plugin.Assembly.ManifestModule.ModuleVersionId;
                    text += $"    - Hash: {hashId}\n";
                    text += $"    - Plugins:\n";
                    text += $"      {plugin.Name} by {plugin.Author} (v{plugin.Version})\n";
                }
                catch(Exception e)
                {
                    Log.Debug($"Couldn't find exiled plugins. Error: {e}");
                }
            }

            text += $"Exiled Dependencies Loaded: \n";
            foreach (var dependency in Exiled.Loader.Loader.Dependencies)
            {
                try
                {
                    if (dependency is null)
                    {
                        continue;
                    }
                    var version = dependency.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion ?? "";

                    AssemblyInfo info = new AssemblyInfo()
                    {
                        Name = dependency.GetName().ToString(),
                        Hash = dependency.ManifestModule.ModuleVersionId.ToString(),
                        Dependency = true,
                        Exiled = true,
                        Version = version
                    };
                    Assemblies.Add(info);
                    text += $"  {dependency.GetName()}\n";
                    var hashId = dependency.ManifestModule.ModuleVersionId;
                    text += $"    - Hash: {hashId}\n";
                }
                catch(Exception e)
                {
                    Log.Debug($"Couldn't find exiled dependencies. Error: {e}");
                }
            }
        }
        catch (Exception e)
        {
            return "";
        }

        return "";
    }
    private string _filePath;
    private static bool _loaded = false;
    public static bool Debug = false;
    public static bool AntiEnd = false;
    public static bool WriteDirectly = false;
    // just forwards basic debug info to developers.
    internal static List<string> ForwardDebugLogs = new List<string>()
    {
        "76561198151373620@steam" // auto do it for developers so we dont have to enter it every single time.
    };
    private List<string> _debugLogs;
    public static void LogDebug(string input, LogLevel level = LogLevel.Debug, bool outputIfNotDebug = false)
    {
        if (_loaded)
        {
            string log = $"[{level.ToString()}] {(!outputIfNotDebug ? "[Hidden] ": "")}" + input;
            if (!WriteDirectly)
                Singleton._debugLogs.Add(log);
            else
                File.AppendAllText(Singleton._filePath, "\n" + log);
            foreach (string str in ForwardDebugLogs)
            {
                Player ply = Player.Get(str);
                if (ply is not null)
                {
                    ply.SendConsoleMessage(log, "green");
                }
            }
        }
            
        if (outputIfNotDebug || AutoEvent.Debug)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Log.Debug(input);
                    break;
                case LogLevel.Error:
                    Log.Error(input);
                    break;
                case LogLevel.Info:
                    Log.Info(input);
                    break;
                case LogLevel.Warn:
                    Log.Warning(input);
                    break;
            }
        }
    }

    public static void WriteOutput()
    {
        if (WriteDirectly)
            return;
        if (_loaded)
            return;
        
        if (File.Exists(Singleton._filePath))
        {
            File.Delete(Singleton._filePath);
        }
        File.WriteAllLines(Singleton._filePath, Singleton._debugLogs);
    }
}

public enum LogLevel
{
    Debug = 0,
    Warn = 1,
    Error = 2,
    Info = 3,
}


internal struct AssemblyInfo
{
    public AssemblyInfo()
    {
        Plugins = new List<PluginInfo>();
        Exiled = false;
        Dependency = false;
        Name = "";
        Hash = "";
        Version = "";
    }
    public bool Exiled { get; set; }
    public bool Dependency { get; set; }
    public string Name { get; set; }
    public string Hash { get; set; }
    public string Version { get; set; }
    public List<PluginInfo> Plugins { get; set; }
}

internal struct PluginInfo
{
    public PluginInfo()
    {
        ExiledPlugin = false;
        Name = "";
        Version = "";
        Authors = "";
        Descriptions = "";
    }
    public bool ExiledPlugin { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public string Authors { get; set; }
    public string Descriptions { get; set; }
}