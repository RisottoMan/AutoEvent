using System;
using System.IO;
using AutoEvent.Commands;
using AutoEvent.Events.EventArgs;
using AutoEvent.Interfaces;
using HarmonyLib;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using Exiled.Loader;
using GameCore;
using MEC;
using PluginAPI.Core;
using Powerups;
using Event = AutoEvent.Interfaces.Event;
using Log = PluginAPI.Core.Log;
using Map = PluginAPI.Core.Map;
using Paths = PluginAPI.Helpers.Paths;
using Player = PluginAPI.Core.Player;
using Server = PluginAPI.Core.Server;
#if EXILED
using Exiled.API.Features;

#endif
namespace AutoEvent
{
#if EXILED
    public class AutoEvent : Plugin<Config, Translation>
    {
        public override System.Version Version => System.Version.Parse(DebugLogger.Version);
        public override string Name => "AutoEvent";
        public override string Author => "Created by KoT0XleB, extended by swd and sky, Co-Maintained by Redforce04";
        public static bool IsPlayedGames;

#else
    public class AutoEvent
    {
        [PluginConfig("Configs/autoevent.yml")]
        public Config Config;

        [PluginConfig("Configs/translation.yml")]
        public Translation Translation;
#endif
        public const bool BetaRelease = false; // todo set beta to false before main release
        /// <summary>
        /// The location of the AutoEvent folder for schematics, music, external events and event config / translations.
        /// </summary>
        /// <example>/home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/</example>
        public static string BaseConfigPath { get; set;}
        public static IEvent ActiveEvent;
        public static AutoEvent Singleton;
        public static Harmony HarmonyPatch;
        public static bool Debug => DebugLogger.Debug;
        EventHandler eventHandler;
        
#if EXILED
        public override void OnEnabled()
#else
        [PluginPriority(LoadPriority.Low)]
        [PluginEntryPoint("AutoEvent", DebugLogger.Version, "An event manager plugin that allows you to run mini-games.",
            "KoT0XleB and Redforce04")]
        void OnEnabled()
#endif
        {
            if (!Config.IsEnabled) return;
            if (BetaRelease)
            {
                Log.Warning("Warning: This release of AutoEvent is a Beta-Release." +
                            " If you encounter any bugs, please reach out to Redforce04 (redforce04) or KoT0XleB (spagettimen) via discord." +
                            " Alternatively, make an issue on our github (https://github.com/KoT0XleB/AutoEvent/). Have fun!");
            }

            // Call Costura first just to ensure dependencies are loaded.
            // Also make sure there isn't anything that needs a dependency in this method.
            CosturaUtility.Initialize();
            
#if !EXILED
            // Root plugin path
            AutoEvent.BaseConfigPath = Path.Combine(Paths.GlobalPlugins.Plugins, "AutoEvent");
#else
            AutoEvent.BaseConfigPath = Path.Combine(Exiled.API.Features.Paths.Configs, "AutoEvent");
#endif
            _startup();
        }

        private void _startup()
        {
            try
            {
                Singleton = this;
                MER.Lite.API.Initialize(AutoEvent.Singleton.Config.SchematicsDirectoryPath, Config.Debug);
                Powerups.API.Initialize();
                SCPSLAudioApi.Startup.SetupDependencies();
                InventoryMenu.API.MenuManager.Init();
                InventoryMenu.API.Log.OnLog += (msg, level) =>
                {
                    DebugLogger.LogDebug($"[InventorySystem] {msg}", (LogLevel)level,
                        level != InventoryMenu.API.Log.LogLevel.Debug);
                };
                #if EXILED
                Exiled.Events.Handlers.Player.Shot += (Exiled.Events.EventArgs.Player.ShotEventArgs ev) =>
                {
                    var args = new ShotEventArgs(Player.Get(ev.Player.ReferenceHub), ev.RaycastHit, ev.Hitbox, ev.Damage);
                    global::AutoEvent.Events.Handlers.Players.OnShot(args);
                    ev.Damage = args.Damage;
                    ev.CanHurt = args.CanHurt;
                };
                #endif
                
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
                    Log.Warning("Could not patch harmony methods.");
                    Log.Debug($"{e}");
                }

                eventHandler = new EventHandler();
                EventManager.RegisterEvents(eventHandler);
                EventManager.RegisterEvents(this);
                SCPSLAudioApi.Startup.SetupDependencies();

                Servers.RemoteAdmin += eventHandler.OnRemoteAdmin;
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


                Event.RegisterInternalEvents();
                Loader.LoadEvents();
                Event.Events.AddRange(Loader.Events);

                DebugLogger.LogDebug(
                    Loader.Events.Count > 0
                        ? $"[ExternalEventLoader] Loaded {Loader.Events.Count} external event{(Loader.Events.Count > 1 ? "s" : "")}."
                        : "No external events were found.", LogLevel.Info, true);
            }
            catch (Exception e)
            {
                Log.Warning("Caught an exception while starting plugin.");
                Log.Debug($"{e}");

            }

            Timing.CallDelayed(3f, () =>
            {
                PermissionSystem.Load();
            });
        }
        public static void CreateDirectoryIfNotExists(string directory, string subPath = "")
        {
            string path = "";
            try
            {
                path = subPath == "" ? directory : Path.Combine(directory, subPath);
                // DebugLogger.LogDebug($"Filepath: {path}");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"An error has occured while trying to create a new directory.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"Path: {path}");
                DebugLogger.LogDebug($"{e}");
            }
        }
#if !EXILED
        [PluginUnload]
        void OnDisabled()
#else
        public override void OnDisabled()
#endif
        {
            Servers.RemoteAdmin -= eventHandler.OnRemoteAdmin;
            eventHandler = null;

            EventManager.UnregisterEvents(this);
            HarmonyPatch.UnpatchAll();
            Singleton = null;
        }

        public void OnEventFinished()
        {
            if (Config.RestartAfterRoundFinish && !DebugLogger.NoRestartEnabled)
            {
                foreach (Player ply in Player.GetPlayers())
                {
                    if (ply.CheckPermission("ev.norestart", out bool isConsole))
                    {
                        ply.ClearBroadcasts();
                        ply.SendBroadcast($"The server is going to restart in 10 seconds. Use the `Ev NoRestart` command to prevent this.", 10);
                    }
                }
                Timing.CallDelayed(7f, () =>
                {
                    if (Config.RestartAfterRoundFinish && !DebugLogger.NoRestartEnabled)
                    {
                        Map.ClearBroadcasts();
                        Map.Broadcast(5, Config.ServerRestartMessage);
                    }
                });
                Timing.CallDelayed(10f, () =>
                {
                    if (Config.RestartAfterRoundFinish && !DebugLogger.NoRestartEnabled)
                    {
                        Server.Restart();
                    }
                });
            }
        }
    }
}
