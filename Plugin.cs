using System;
using System.IO;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using HarmonyLib;

namespace AutoEvent
{
    public class AutoEvent : Plugin<Config, Translation>
    {
        public override string Name => "AutoEvent";
        public override string Author => "Created by KoT0XleB, extended by swd and sky";
        public override Version Version => new Version(8, 1, 0);
        public static IEvent ActiveEvent = null;
        public static AutoEvent Singleton;
        public static Harmony HarmonyPatch;
        public override void OnEnabled()
        {
            Singleton = this;
            HarmonyPatch = new Harmony("autoevent");
            HarmonyPatch.PatchAll();
            Event.RegisterEvents();

            if (!Config.IsEnabled) return;

            if (!Directory.Exists(Path.Combine(Paths.Configs, "Music"))) Directory.CreateDirectory(Path.Combine(Paths.Configs, "Music"));

            Exiled.Events.Handlers.Server.RestartingRound += OnRestarting;
            Exiled.Events.Handlers.Map.Decontaminating += OnDecontamination;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            HarmonyPatch.UnpatchAll();
            Singleton = null;

            Exiled.Events.Handlers.Server.RestartingRound -= OnRestarting;
            Exiled.Events.Handlers.Map.Decontaminating -= OnDecontamination;
            base.OnDisabled();
        }

        private void OnRestarting()
        {
            if (ActiveEvent == null) return;

            Extensions.StopAudio();
            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
        }

        private void OnDecontamination(DecontaminatingEventArgs ev)
        {
            if (ActiveEvent != null)
            {
                ev.IsAllowed = false;
            }
        }
    }
}
