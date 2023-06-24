using System;
using System.IO;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using HarmonyLib;

namespace AutoEvent
{
    public class AutoEvent : Plugin<Config, Translation>
    {
        public override string Name => "AutoEvent";
        public override string Author => "Created by KoT0XleB, extended by swd";
        public override Version Version => new Version(7, 1, 0);
        public static IEvent ActiveEvent = null;
        public static AutoEvent Singleton;
        public static Harmony HarmonyPatch;
        public override void OnEnabled()
        {
            Singleton = this;

            HarmonyPatch = new Harmony("autoevent");
            HarmonyPatch.PatchAll();

            if (!Config.IsEnabled) return;
            // Checking for the music directory
            if (!Directory.Exists(Path.Combine(Paths.Configs, "Music"))) Directory.CreateDirectory(Path.Combine(Paths.Configs, "Music"));
            

            Exiled.Events.Handlers.Server.RestartingRound += OnRestarting;
        }
        public override void OnDisabled()
        {
            HarmonyPatch.UnpatchAll();
            Singleton = null;

            Exiled.Events.Handlers.Server.RestartingRound -= OnRestarting;
        }
        private void OnRestarting()
        {
            if (ActiveEvent == null) return;

            Extensions.StopAudio();
        }
    }
}
