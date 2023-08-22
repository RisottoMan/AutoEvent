using System.IO;
using AutoEvent.Interfaces;
using HarmonyLib;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Helpers;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent
{
    public class AutoEvent
    {
        public static IEvent ActiveEvent = null;
        public static AutoEvent Singleton;
        public static Harmony HarmonyPatch;
        EventHandler eventHandler;

        [PluginConfig("configs/autoevent.yml")]
        public Config Config;

        [PluginConfig("configs/translation.yml")]
        public Translation Translation;

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("AutoEvent-NWApi", "8.2.7", "A plugin that allows you to run mini-games.", "KoT0XleB")]
        void OnEnabled()
        {
            if (!Config.IsEnabled) return;

            Singleton = this;
            HarmonyPatch = new Harmony("autoevent-nwapi");
            HarmonyPatch.PatchAll();

            Event.RegisterEvents();
            EventManager.RegisterEvents(this);
            SCPSLAudioApi.Startup.SetupDependencies();

            eventHandler = new EventHandler();
            Servers.RemoteAdmin += eventHandler.OnRemoteAdmin;

            if (!Directory.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "Music")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.GlobalPlugins.Plugins, "Music"));
            }

            if (!Directory.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "Schematics")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.GlobalPlugins.Plugins, "Schematics"));
            }
        }

        [PluginUnload]
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
