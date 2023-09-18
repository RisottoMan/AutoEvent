using System.IO;
using AutoEvent.Interfaces;
using HarmonyLib;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using GameCore;
using PluginAPI.Core;
using Event = AutoEvent.Interfaces.Event;
using Paths = PluginAPI.Helpers.Paths;
using Server = PluginAPI.Core.Server;
#if EXILED
using Exiled.API.Features;

#endif
namespace AutoEvent
{
#if EXILED
    public class AutoEvent : Plugin<Config, Translation>
    {
        public override System.Version Version => new System.Version(9, 0, 2);
        public override string Name => "AutoEvent";
        public override string Author => "Created by KoT0XleB, extended by swd and sky";
        public static bool IsPlayedGames;

#else
    public class AutoEvent
    {
#endif
        /// <summary>
        /// The location of the AutoEvent folder for schematics, music, external events and event config / translations.
        /// </summary>
        /// <example>/home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/</example>
        public static string BaseConfigPath { get; set;}
        public static IEvent ActiveEvent;
        public static AutoEvent Singleton;
        public static Harmony HarmonyPatch;
        public static bool Debug => DebugLogger.Debug;
        public static bool IsFriendlyFireEnabledByDefault { get; set; }
        EventHandler eventHandler;
        
#if !EXILED
        [PluginConfig("configs/autoevent.yml")]
#endif
        public Config Config;

#if !EXILED
        [PluginConfig("configs/translation.yml")]
#endif
        public Translation Translation;

#if !EXILED
        [PluginPriority(LoadPriority.Low)]
        [PluginEntryPoint("AutoEvent", "9.0.2", "A plugin that allows you to run mini-games.", "KoT0XleB")]
#endif
        void OnEnabled()
        {
            if (!Config.IsEnabled) return;

            IsFriendlyFireEnabledByDefault = Server.FriendlyFire;
            Singleton = this;
            var debugLogger = new DebugLogger();
            DebugLogger.Debug = Config.Debug;
            if (DebugLogger.Debug)
            {
                DebugLogger.LogDebug($"Debug Mode Enabled", LogLevel.Info, true);
            }
            HarmonyPatch = new Harmony("autoevent-nwapi");
            HarmonyPatch.PatchAll();

            EventManager.RegisterEvents(this);
            SCPSLAudioApi.Startup.SetupDependencies();

            eventHandler = new EventHandler();
            Servers.RemoteAdmin += eventHandler.OnRemoteAdmin;


#if !EXILED
            // Root plugin path
            AutoEvent.BaseConfigPath = Path.Combine(Paths.GlobalPlugins.Plugins, "AutoEvent");
#endif
            CreateDirectoryIfNotExists(BaseConfigPath);
            CreateDirectoryIfNotExists(BaseConfigPath, "Events");
            CreateDirectoryIfNotExists(BaseConfigPath, "Music");
            CreateDirectoryIfNotExists(BaseConfigPath, "Schematics");
            CreateDirectoryIfNotExists(BaseConfigPath, "Configs");

            Event.RegisterInternalEvents();
            Loader.LoadEvents();
            Event.Events.AddRange(Loader.Events);
            
            DebugLogger.LogDebug(Loader.Events.Count > 0 ? $"[ExternalEventLoader] Loaded {Loader.Events.Count} external event{(Loader.Events.Count > 1 ? "s" : "")}." : "No external events were found.", LogLevel.Info, true);
        }

        public static void CreateDirectoryIfNotExists(string directory, string subPath = "")
        {
            string path = subPath == "" ? directory : Path.Combine(directory, subPath);  
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
#if !EXILED
        [PluginUnload]
#endif
        void OnDisabled()
        {
            Servers.RemoteAdmin -= eventHandler.OnRemoteAdmin;
            eventHandler = null;

            EventManager.UnregisterEvents(this);
            HarmonyPatch.UnpatchAll();
            Singleton = null;
        }
    }
}
