using System.IO;
using AutoEvent.Interfaces;
using HarmonyLib;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using GameCore;
using Event = AutoEvent.Interfaces.Event;
using Paths = PluginAPI.Helpers.Paths;
#if EXILED
using Exiled.API.Features;

#endif
namespace AutoEvent
{
#if EXILED
    public class AutoEvent : Plugin<Config, Translation>
    {
        public override string Name => "AutoEvent";
        public override string Author => "Created by KoT0XleB, extended by swd and sky";
        public static bool IsPlayedGames;

#else
    public class AutoEvent
    {
#endif
        public static IEvent ActiveEvent;
        public static AutoEvent Singleton;
        public static Harmony HarmonyPatch;
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
        [PluginEntryPoint("AutoEvent-NWApi", "8.2.7", "A plugin that allows you to run mini-games.", "KoT0XleB")]
#endif
        void OnEnabled()
        {
            if (!Config.IsEnabled) return;

            Singleton = this;
            HarmonyPatch = new Harmony("autoevent-nwapi");
            HarmonyPatch.PatchAll();

            EventManager.RegisterEvents(this);
            SCPSLAudioApi.Startup.SetupDependencies();

            eventHandler = new EventHandler();
            Servers.RemoteAdmin += eventHandler.OnRemoteAdmin;

            Event.RegisterEvents();
            
            // Load External Events.
            if (!Directory.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "Events")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.GlobalPlugins.Plugins, "Events"));
            }
            Loader.LoadEvents();
            Event.Events.AddRange(Loader.Events);
            
            PluginAPI.Core.Log.Info(Loader.Events.Count > 0 ? $"[ExternalEventLoader] Loaded {Loader.Events.Count} external event{(Loader.Events.Count > 1 ? "s" : "")}." : "No external events were found.");
            if (!Directory.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "Music")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.GlobalPlugins.Plugins, "Music"));
            }

            if (!Directory.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "Schematics")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.GlobalPlugins.Plugins, "Schematics"));
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
