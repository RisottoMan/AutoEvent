using System;
using System.Collections.Generic;
using PlayerRoles;
using System.Linq;
using UnityEngine;
using PluginAPI.Events;
using PluginAPI.Core;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Football
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Football";
        public override string Description { get; set; } = "Score 3 goals to win";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "football";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig] 
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "Football", 
            Position = new Vector3(76f, 1026.5f, -43.68f),
            IsStatic = false
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Football.ogg", 
            Volume = 5
        };
        protected override float PostRoundDelay { get; set; } = 10f;
        protected override float FrameDelayInSeconds { get; set; } = 0.3f;
        private EventHandler _eventHandler { get; set; }
        private TimeSpan _remainingTime;
        private int _bluePoints;
        private int _redPoints;
        private GameObject _ball;
        private List<GameObject> _triggers;
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Players.DropItem += _eventHandler.OnDropItem;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Players.DropItem -= _eventHandler.OnDropItem;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _remainingTime = new TimeSpan(0,0,Config.MatchDurationInSeconds);
            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    Extensions.SetRole(player, RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }
                count++;
            }
            _bluePoints = 0;
            _redPoints = 0;
            _ball = new GameObject();
            _triggers = new List<GameObject>();

            foreach (var gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Trigger": { _triggers.Add(gameObject); } break;
                    case "Ball": { _ball = gameObject; _ball.AddComponent<BallComponent>(); } break;
                }
            }
        }

        protected override bool IsRoundDone()
        {
            // Both teams have less than 3 points &&
            // The elapsed time is under 3 minutes &&
            // Both Teams have at least 1 player 
            return !(_bluePoints < Config.PointsToWin && _redPoints < Config.PointsToWin && EventTime.TotalSeconds < Config.MatchDurationInSeconds &&
                     Player.GetPlayers().Count(r => r.IsNTF) > 0 &&
                     Player.GetPlayers().Count(r => r.Team == Team.ClassD) > 0);
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
                        text += Translation.BlueTeam;
                    }
                    else
                    {
                        text += Translation.RedTeam;
                    }

                    player.ClearBroadcasts();
                    player.SendBroadcast(text + Translation.TimeLeft.
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
                Extensions.Broadcast($"{Translation.BlueWins}", 10);
            }
            else if (_redPoints > _bluePoints)
            {
                Extensions.Broadcast($"{Translation.RedWins}", 10);
            }
            else
            {
                Extensions.Broadcast($"{Translation.Draw.Replace("{BluePnt}", $"{_bluePoints}").Replace("{RedPnt}", $"{_redPoints}")}", 3);
            }
        }
    }
}
