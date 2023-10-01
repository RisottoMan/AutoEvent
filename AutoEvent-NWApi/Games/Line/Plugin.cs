using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Line
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.LineTranslate.LineName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.LineTranslate.LineDescription;
        public override string Author { get; set; } = "Logic_Gun";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.LineTranslate.LineCommandName;
        [EventConfig]
        public LineConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Line", Position = new Vector3(76f, 1026.5f, -43.68f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "LineLite.ogg", Volume = 10, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private LineTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.LineTranslate;
        private readonly int _hardCountsLimit = 8;
        private Dictionary<int, SchematicObject> _hardGameMap;
        private TimeSpan _timeRemaining;
        private int _hardCounts;
        // todo - revamp configs for this

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
            _timeRemaining = new TimeSpan(0, 2, 0);
            _hardGameMap = new Dictionary<int, SchematicObject>();
            _hardCounts = 0;
            
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.Loadouts);
                player.Position = MapInfo.Map.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            }

        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            foreach (var block in MapInfo.Map.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "DeadZone": block.AddComponent<LineComponent>().Init(this, ObstacleType.MiniWalls); break;
                    case "DeadWall": block.AddComponent<LineComponent>().Init(this, ObstacleType.Wall); break;
                    case "Line": block.AddComponent<LineComponent>().Init(this, ObstacleType.Ground); break;
                    case "Shield": GameObject.Destroy(block); break;
                }
            }
        }

        protected override void ProcessFrame()
        {
            Extensions.Broadcast(Translation.LineCycle.Replace("{name}", Name).
                Replace("{time}", $"{_timeRemaining.Minutes:00}:{_timeRemaining.Seconds:00}").
                Replace("{count}", $"{Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD)}"), 10);

            if (EventTime.Seconds == 30 && _hardCounts < _hardCountsLimit)
            {
                if (_hardCounts == 0)
                {
                    Extensions.StopAudio();
                    Extensions.PlayAudio("LineHard.ogg", 10, true, Name);
                }

                try
                {
                    var map_hard = Extensions.LoadMap("HardLine", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
                    _hardGameMap.Add(_hardCounts, map_hard);
                }
                catch(Exception ex)
                {
                    DebugLogger.LogDebug($"An error has occured while processing frame.", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"{ex}", LogLevel.Debug);

                }

                _hardCounts++;
            }
            _timeRemaining -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        }

        protected override bool IsRoundDone()
        {
            // At least 2 players &&
            // Time is smaller than 2 minutes (+countdown)
            return !(Player.GetPlayers().Count(r => r.Role != AutoEvent.Singleton.Config.LobbyRole) > 1 && EventTime.TotalSeconds < 120);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Role !=AutoEvent.Singleton.Config.LobbyRole) > 1)
            {
                Extensions.Broadcast(Translation.LineMorePlayers.
                    Replace("{name}", Name).
                    Replace("{count}", $"{Player.GetPlayers().Count(r => r.Role != AutoEvent.Singleton.Config.LobbyRole)}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Role != AutoEvent.Singleton.Config.LobbyRole) == 1)
            {
                Extensions.Broadcast(Translation.LineWinner.
                    Replace("{name}", Name).
                    Replace("{winner}", Player.GetPlayers().First(r => r.Role != AutoEvent.Singleton.Config.LobbyRole).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.LineAllDied, 10);
            }
        }

        protected override void OnCleanup()
        {
            foreach (var map in _hardGameMap.Values)
                Extensions.UnLoadMap(map);
        }
        
    }
}