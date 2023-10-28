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
using CedMod.Handlers;
using PlayerRoles;
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
                    var text = string.Empty;
                    if (Vector3.Distance(_ball.transform.position, player.Position) < 2)
                    {
                        _ball.gameObject.TryGetComponent(out Rigidbody rig);
                        rig.AddForce(player.ReferenceHub.transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
                    }

                    if (player.Team == Team.FoundationForces)
                    {
                        text += Translation.FootballBlueTeam;
                    }
                    else
                    {
                        text += Translation.FootballRedTeam;
                    }

                    player.ClearBroadcasts();
                    player.SendBroadcast(text + Translation.FootballTimeLeft.
                        Replace("{BluePnt}", $"{_bluePoints}").
                        Replace("{RedPnt}", $"{_redPoints}").
                        Replace("{time}", time), 1);
                }

                if (Vector3.Distance(_ball.transform.position, _triggers.ElementAt(0).transform.position) < 3)
                {
                    _ball.transform.position = MapInfo.Map.Position + new Vector3(0, 2.5f, 0);
                    _ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _redPoints++;
                }

                if (Vector3.Distance(_ball.transform.position, _triggers.ElementAt(1).transform.position) < 3)
                {
                    _ball.transform.position = MapInfo.Map.Position + new Vector3(0, 2.5f, 0);
                    _ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _bluePoints++;
                }

                if (_ball.transform.position.y < MapInfo.Map.Position.y - 10f)
                {
                    _ball.transform.position = MapInfo.Map.Position + new Vector3(0, 2.5f, 0);
                    _ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }

                _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        }

        protected override void OnFinished()
        {
            if (_bluePoints > _redPoints)
            {
                Extensions.Broadcast($"{Translation.FootballBlueWins}", 10);
            }
            else if (_redPoints > _bluePoints)
            {
                Extensions.Broadcast($"{Translation.FootballRedWins}", 10);
            }
            else
            {
                Extensions.Broadcast($"{Translation.FootballDraw.Replace("{BluePnt}", $"{_bluePoints}").Replace("{RedPnt}", $"{_redPoints}")}", 3);
            }
        }
    }