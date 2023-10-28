// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Plugin.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 1:50 AM
//    Created Date:     10/28/2023 1:50 AM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.GhostBusters.Configs;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.GhostBusters;

public class Plugin : Event, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate.GhostBustersName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate.GhostBustersDescription;
        public override string Author { get; set; } = "Redforce04 and Riptide";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate.GhostBustersCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig] public GhostBustersConfig Config { get; set; }
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Ghostbusters.ogg", Volume = 5, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        protected override float FrameDelayInSeconds { get; set; } = 1f;
        private EventHandler EventHandler { get; set; }
        private GhostBustersTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.GhostBustersTranslate;
        private TimeSpan _remainingTime;
        public enum Stage { Prep, PreMidnight, Midnight }
        protected override void RegisterEvents()
        {

            EventHandler = new EventHandler(this);
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

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
        protected override void OnStart()
        {
            _remainingTime = new TimeSpan(0,0,Config.MatchDurationInSeconds);
            
        }

        protected override bool IsRoundDone()
        {
            
        }

        protected override void ProcessFrame()
        {
            var time = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";
                foreach (Player player in Player.GetPlayers())
                {
                    
                }

                var a = PluginAPI.Core.Map.Rooms.First(x => x.Name == RoomName.HczCheckpointA);
                var b = PluginAPI.Core.Map.Rooms.First(x => x.Name == RoomName.HczCheckpointB);
                //a.ApiRoom
                var nameTag = a.gameObject.GetComponentInChildren<DoorNametagExtension>().TargetDoor.ServerChangeLock() ? name.GetName : null;

                _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        }

        protected override void OnFinished()
        {
            int ghosts = Player.GetPlayers().Count(x => !x.HasLoadout(Config.MeleeLoadout) && !x.HasLoadout(Config.SniperLoadout) &&
                                                        !x.HasLoadout(Config.TankLoadout));
            if (ghosts > 0)
            {
                Map.Broadcast(10, Translation.GhostBustersGhostsWin);
            }
            else
            {
                Map.Broadcast(10, Translation.GhostBustersHuntersWin);
            }
        }
    }