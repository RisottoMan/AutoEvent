
using System;
using System.Collections.Generic;
using MEC;
using PlayerRoles;
using System.Linq;
using UnityEngine;
using AutoEvent.Games.Football.Features;
using PluginAPI.Events;
using PluginAPI.Core;
using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Football
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.FootballTranslate.FootballName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.FootballTranslate.FootballDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "ball";
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Football", Position = new Vector3(76f, 1026.5f, -43.68f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Football.ogg", Volume = 5, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        protected override float FrameDelayInSeconds { get; set; } = 0.3f;
        private EventHandler EventHandler { get; set; }
        private FootballTranslate Translation { get; set; }
        private int _bluePoints;
        private int _redPoints;
        private GameObject _ball;
        private List<GameObject> _triggers;
        protected override void RegisterEvents()
        {
            Translation = new FootballTranslate();
            EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Players.DropItem += EventHandler.OnDropItem;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Players.DropItem -= EventHandler.OnDropItem;

            EventHandler = null;
        }

        protected override void OnStart()
        {
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
            return !(_bluePoints < 3 && _redPoints < 3 && EventTime.TotalSeconds < 180 &&
                     Player.GetPlayers().Count(r => r.IsNTF) > 0 &&
                     Player.GetPlayers().Count(r => r.Team == Team.ClassD) > 0);
        }

        protected override void ProcessFrame()
        {
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
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
                        Replace("{eventTime}", time), 1);
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
}
