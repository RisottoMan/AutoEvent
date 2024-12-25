using System;
using System.IO;
using HarmonyLib;
using AutoEvent.API;
using AutoEvent.API.Season;
using Server = PluginAPI.Core.Server;
using Exiled.API.Features;

namespace AutoEvent;
public class AutoEvent : Plugin<Config>
{
    public override string Name => "AutoEvent";
    public override string Author => "Created by a large community of programmers, map builders and just ordinary people, under the leadership of RisottoMan.";
    public override Version Version => Version.Parse("9.10.0");
    public override Version RequiredExiledVersion => new(9, 0, 0);
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
            SCPSLAudioApi.Startup.SetupDependencies();
            
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
                                     $"{Config.MusicDirectoryPath}\n" + 
                                     $"{Config.ExternalEventsDirectoryPath}\n" +
                                     $"{Config.EventConfigsDirectoryPath}\n");
                CreateDirectoryIfNotExists(BaseConfigPath);
                CreateDirectoryIfNotExists(Config.SchematicsDirectoryPath);
                CreateDirectoryIfNotExists(Config.MusicDirectoryPath);
                CreateDirectoryIfNotExists(Config.ExternalEventsDirectoryPath);
                CreateDirectoryIfNotExists(Config.EventConfigsDirectoryPath);
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"An error has occured while trying to initialize directories.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}");
            }

            // By mistake, I included all the open source maps in the archive Schematics.tar.gz
            string opensourcePath = Path.Combine(Config.SchematicsDirectoryPath, "All Source maps");
            if (Directory.Exists(opensourcePath))
            {
                Directory.Delete(opensourcePath, true);
            }

            _eventHandler = new EventHandler(this);
            EventManager = new EventManager();
            EventManager.RegisterInternalEvents();
            SeasonMethod.GetSeasonStyle();
            
            DebugLogger.LogDebug($"The mini-games are loaded.", LogLevel.Info, true);
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug("Caught an exception while starting plugin.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
        
        base.OnEnabled();
    }
    
    public static void CreateDirectoryIfNotExists(string directory, string subPath = "")
    {
        string path = "";
        try
        {
            path = subPath == "" ? directory : Path.Combine(directory, subPath);
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
    
    public override void OnDisabled()
    {
        _eventHandler = null;

        HarmonyPatch.UnpatchAll();
        EventManager = null;
        Singleton = null;
        
        base.OnDisabled();
    }
}