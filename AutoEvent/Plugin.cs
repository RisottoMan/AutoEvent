using System;
using System.IO;
using HarmonyLib;
using AutoEvent.API;
using Exiled.API.Features;

namespace AutoEvent;
public class AutoEvent : Plugin<Config>
{
    public override string Name => "AutoEvent";
    public override string Author => "Created by a large community of programmers, map builders and just ordinary people, under the leadership of RisottoMan. MapEditorReborn for 14.1 port by Sakred_";
    public override Version Version => Version.Parse("9.11.2");
    public override Version RequiredExiledVersion => new(9, 6, 0);
    public static string BaseConfigPath { get; set;}
    public static AutoEvent Singleton;
    public static Harmony HarmonyPatch;
    public static EventManager EventManager;
    private EventHandler _eventHandler;
    
    public override void OnEnabled()
    {
        if (!Config.IsEnabled) return;

        CosturaUtility.Initialize();
        
        BaseConfigPath = Path.Combine(Paths.Configs, "AutoEvent");
        
        try
        {
            Singleton = this;
            
            if (Config.IgnoredRoles.Contains(Config.LobbyRole))
            {
                DebugLogger.LogDebug("The Lobby Role is also in ignored roles. This will break the game if not changed. The plugin will remove the lobby role from ignored roles.", LogLevel.Error, true);
                Config.IgnoredRoles.Remove(Config.LobbyRole);
            }

            FriendlyFireSystem.IsFriendlyFireEnabledByDefault = Server.FriendlyFire;

            var debugLogger = new DebugLogger(Config.AutoLogDebug);
            DebugLogger.Debug = Config.Debug;
            if (DebugLogger.Debug)
            {
                DebugLogger.LogDebug($"Debug Mode Enabled", LogLevel.Info, true);
            }
            
            try
            {
                HarmonyPatch = new Harmony("autoevent");
                HarmonyPatch.PatchAll();
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug("Could not patch harmony methods.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}");
            }

            try
            {
                DebugLogger.LogDebug($"Base Conf Path: {BaseConfigPath}");
                DebugLogger.LogDebug($"Configs paths: \n" +
                                     $"{Config.SchematicsDirectoryPath}\n" +
                                     $"{Config.MusicDirectoryPath}\n");
                CreateDirectoryIfNotExists(BaseConfigPath);
                CreateDirectoryIfNotExists(Config.SchematicsDirectoryPath);
                CreateDirectoryIfNotExists(Config.MusicDirectoryPath);
                
                // temporarily
                DeleteDirectoryAndFiles(Path.Combine(BaseConfigPath, "Configs"));
                DeleteDirectoryAndFiles(Path.Combine(BaseConfigPath, "Events"));
                DeleteDirectoryAndFiles(Path.Combine(Path.Combine(BaseConfigPath, "Schematics"), "All Source maps"));
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"An error has occured while trying to initialize directories.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}");
            }

            _eventHandler = new EventHandler(this);
            EventManager = new EventManager();
            EventManager.RegisterInternalEvents();
            ConfigManager.LoadConfigsAndTranslations();
            
            DebugLogger.LogDebug($"The mini-games are loaded.", LogLevel.Info, true);
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("Caught an exception while starting plugin.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
        
        base.OnEnabled();
    }
    
    private static void CreateDirectoryIfNotExists(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("An error has occured while trying to create a new directory.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"Path: {path}\n{e}");
        }
    }
    
    private static void DeleteDirectoryAndFiles(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("An error has occured while trying to delete a directory.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"Path: {path}\n{e}");
        }
    }
    
    public override void OnDisabled()
    {
        _eventHandler = null;

        HarmonyPatch.UnpatchAll();
        EventManager = null;
        Singleton = null;
        
        base.OnDisabled();
    }
}