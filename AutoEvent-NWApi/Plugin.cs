using System;
using System.IO;
using AutoEvent.Interfaces;
using HarmonyLib;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Helpers;
using PluginAPI.Events;
using LightContainmentZoneDecontamination;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent
{
    public class AutoEvent
    {
        public static IEvent ActiveEvent = null;
        public static AutoEvent Singleton;
        public static Harmony HarmonyPatch;
        public static int CountOfPlayedGames;

        [PluginConfig("configs/autoevent.yml")]
        public Config Config;

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("AutoEvent-NWApi", "8.2.3", "A plugin that allows you to run mini-games.", "KoT0XleB")]
        void OnEnabled()
        {
            Log.Info("AutoEvent-NWApi started :D");

            Singleton = this;
            CountOfPlayedGames = 0;
            HarmonyPatch = new Harmony("autoevent");
            HarmonyPatch.PatchAll();
            Event.RegisterEvents();
            EventManager.RegisterEvents(this);

            if (!Config.IsEnabled) return;

            if (!Directory.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "Music")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.GlobalPlugins.Plugins, "Music"));
            }

            if (!Directory.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "Schematics")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.GlobalPlugins.Plugins, "Schematics"));
            }
        }

        [PluginUnload] // it works???
        void OnDisabled()
        {
            Log.Info("Unload plugin");
            EventManager.UnregisterEvents(this);
            HarmonyPatch.UnpatchAll();
            Singleton = null;
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        public void OnRestarting()
        {
            if (ActiveEvent == null || CountOfPlayedGames == 0) return;

            Extensions.StopAudio();
            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
        }

        [PluginEvent(ServerEventType.LczDecontaminationStart)]
        public void OnDecontamination(LczDecontaminationStartEvent ev)
        {
            if (ActiveEvent == null) return;

            DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
        }
    }
}
