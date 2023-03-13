using System;
using System.IO;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;

namespace AutoEvent
{
    public class AutoEvent : Plugin<Config, Translation>
    {
        public override string Name => "AutoEvent";
        public override string Author => "Ported to Exiled [by KoToXleB]";
        public override Version Version => new Version(1, 0, 7);
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
        }
        public override void OnDisabled()
        {
            HarmonyPatch.UnpatchAll();
            Singleton = null;
        }
    }
}
