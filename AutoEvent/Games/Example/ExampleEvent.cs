// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ExampleEvent.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 11:44 AM
//    Created Date:     09/13/2023 11:44 AM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Example
{
    // Do not use IInternalEvent on your events.
    // It is only for the main events that are included with AutoEvent.
    public class ExampleEvent : Event, IEventMap, IEventSound, IInternalEvent
    {

        // Set the info for the event.
        public override string Name { get; set; } = "Example"; // It is recommended to use a translation for everything but author.
        public override string Description { get; set; } = "An example event based on the battle event.";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "example";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        
        // Make sure you set this to true. Otherwise you must register your plugin via Exiled or NWApi manually.
        // Add the event to AutoEvent.Events to manually register it.
        public override bool AutoLoad { get; protected set; } = false; // Because this is the example event, we set it to true so users dont see the example event.
        

        // Users can use this preset instead of the default config if they choose. 
        // Users can also make their own config presets. They are stored in a preset folder inside the event config folder.
        [EventConfigPreset]
        public ExampleConfig BigPeople => Presets.BigPeople();
        
        [EventConfigPreset]
        public ExampleConfig SingleLoadout => Presets.SingleLoadout();
        
        [EventConfig]
        public ExampleConfig Config { get; set; }

        // Map Info can be inherited as long as the event inherits IEventMap.
        // MapInfo.Map is the Schematic Object for the map.
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "Battle", 
                Position = new Vector3(6f, 1030f, -43.5f), 
                MapRotation = new Quaternion(), 
                Scale = new Vector3(1,1,1), 
                // If this is set to false, you can manually spawn the map via base.SpawnMap();
                SpawnAutomatically = true};

        // Sound Info can be inherited as long as the event inherits IEventSound.
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "MetalGearSolid.ogg", 
                Volume = 10, 
                Loop = false, 
                // If this is set to false, you can manually start the audio the map via base.StartAudio();
                StartAutomatically = true
            };
        
        // Define the fields/properties here. Make sure to set them, in OnStart() or OnRegisteringEvents()
        // Define the properties that may be used by this event, or by its handler class.
        private EventHandler EventHandler { get; set; }
        private ExampleTranslate Translation { get; set; }
        
        // Define the fields that will only be used inside this event class.
        private List<GameObject> _workstations;
        
        // Events only need to be registered when the GameMode Event is being run.
        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

        // Events are unregistered when the GameMode Event is finished.
        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropItem -= EventHandler.OnDropItem;
            Players.DropAmmo -= EventHandler.OnDropAmmo;
            EventHandler = null;
        }

        // Define what you want to happen when the even is started / run.
        protected override void OnStart()
        {

            int count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    Extensions.SetRole(player, RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }

                count++;
                
                // You can use either a single loadout or a list of loadouts. 
                // The list is good because it allows users to add a chance to each loadout.
                // Only one loadout will be assigned.
                player.GiveLoadout(Config.Loadouts, LoadoutFlags.IgnoreGodMode);
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = player.Items.First(); });
            }

        }

        // Override this coroutine if you want a start countdown.
        // The coroutine runs automatically. Nothing else will run until this is done.
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            // Count down the time until start. we use a 20 second timer for this.
            for (int time = 20; time > 0; time--)
            {
                Extensions.Broadcast(Translation.BattleTimeLeft.Replace("{time}", $"{time}"), 5);
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }

        // This is executed after the start countdown is finished. This is good for removing start barriers, spawning players, etc... 
        protected override void CountdownFinished()
        {
            // Once the countdown has ended, we need to destroy the walls, and add workstations.
            _workstations = new List<GameObject>();
            foreach (var gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Wall": { GameObject.Destroy(gameObject); } break;
                    case "Workstation": { _workstations.Add(gameObject); } break;
                }
            }
        }


        // It is recommended to not override this, but it can be overriden as necessary. 
        /* protected override IEnumerator<float> RunGameCoroutine()
        {
            // Note the structure of how things are called. Its an abstracted while loop.
            // for debugging, look at the debug commands. it is helpful for testing events. 
            while (!IsRoundDone() || DebugLogger.AntiEnd)
            {
                if (KillLoop)
                {
                    yield break;
                }
                try
                {
                    ProcessFrame();                
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"Caught an exception at Event.ProcessFrame().", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                }

                EventTime += TimeSpan.FromSeconds(FrameDelayInSeconds);
                yield return Timing.WaitForSeconds(this.FrameDelayInSeconds);
            }
            yield break;
        }*/

        // Use this to determine if the round is done. If this is false, ProcessFrame() is called once a second.
        protected override bool IsRoundDone()
        {
            // When Getting Players -> Use Player.GetPlayers.Count() Not Player.Count. -
            // This has a patch on it to prevent players who are tutorial / blacklisted classes from being counted.
            
            // Round finishes when either team has no more players.
            return !EndConditions.TeamHasMoreThanXPlayers(Team.FoundationForces,0) ||
                   !EndConditions.TeamHasMoreThanXPlayers(Team.ChaosInsurgency,0);
        }

        // How long between each ProcessFrame()
        protected override float FrameDelayInSeconds { get; set; } = 1f;
        // Use to trigger events.
        protected override void ProcessFrame()
        {
            // While the round isn't done, this will be called once a second. You can make the call duration faster / slower by changing FrameDelayInSeconds.
            // While the round is still going, broadcast the current round stats.
            var text = Translation.BattleCounter;
            text = text.Replace("{FoundationForces}", $"{Player.GetPlayers().Count(r => r.Team == Team.FoundationForces)}");
            text = text.Replace("{ChaosForces}", $"{Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency)}");
            text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

            Extensions.Broadcast(text, 1);
        }

        // This executes only if the event finishes. If the event is stopped. OnStop will be called instead.
        protected override void OnFinished()
        {
            // Once the round is finished, broadcast the winning team (either mtf or chaos in this case.)
            // If the round is stopped, this wont be called. Instead use OnStop to broadcast either winners, or that nobody wins because the round was stopped.
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.BattleCiWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 3);
            }
            else // if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.BattleMtfWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }        
        }

        // Can be used to broadcast that the event is stopping. Can also be used to stop extra coroutines.
        protected override void OnStop()
        {
            base.OnStop();
        }

        // How long to wait before cleaning up the map, despawning players, etc...
        protected override float PostRoundDelay { get; set; } = 10f;

        
        // Always called. The map will automatically be despawned, audio will automatically be stopped.
        protected override void OnCleanup()
        {
            // 10 seconds after finishing the round or once the round is stopped, this will be called.
            // If 10 seconds is too long, you can change PostRoundDelay to make it faster or shorter.
            // We can cleanup extra workstations that we spawned in. 
            // The map will be cleaned up for us, as well as items, ragdolls, and sound.
            foreach (var bench in _workstations)
                GameObject.Destroy(bench);
        }


    }
}
