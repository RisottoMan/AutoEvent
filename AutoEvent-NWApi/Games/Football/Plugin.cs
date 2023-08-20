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
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Football
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.FootballName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.FootballDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Football";
        public override string CommandName { get; set; } = "ball";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Players.DropItem += _eventHandler.OnDropItem;

            OnEventStarted();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Players.DropItem -= _eventHandler.OnDropItem;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 3, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Football.ogg", 5, true, Name);

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    Extensions.SetRole(player, RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;
            }

            Timing.RunCoroutine(OnEventRunning(), "glass_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;

            int bluePoints = 0;
            int redPoints = 0;
            GameObject ball = new GameObject();
            List<GameObject> triggers = new List<GameObject>();

            foreach (var gameObject in GameMap.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Trigger": { triggers.Add(gameObject); } break;
                    case "Ball": { ball = gameObject; ball.AddComponent<BallComponent>(); } break;
                }
            }

            while (bluePoints < 3 && redPoints < 3 && EventTime.TotalSeconds > 0 && Player.GetPlayers().Count(r => r.IsNTF) > 0 && Player.GetPlayers().Count(r => r.Team == Team.ClassD) > 0)
            {
                var time = $"{EventTime.Minutes}:{EventTime.Minutes}";
                foreach (Player player in Player.GetPlayers())
                {
                    var text = string.Empty;
                    if (Vector3.Distance(ball.transform.position, player.Position) < 2)
                    {
                        ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                        rig.AddForce(player.ReferenceHub.transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
                    }

                    if (player.Team == Team.FoundationForces)
                    {
                        text += translation.FootballBlueTeam;
                    }
                    else
                    {
                        text += translation.FootballRedTeam;
                    }

                    player.ClearBroadcasts();
                    player.SendBroadcast(text + translation.FootballTimeLeft.
                        Replace("{BluePnt}", $"{bluePoints}").
                        Replace("{RedPnt}", $"{redPoints}").
                        Replace("{eventTime}", time), 1);
                }

                if (Vector3.Distance(ball.transform.position, triggers.ElementAt(0).transform.position) < 3)
                {
                    ball.transform.position = GameMap.Position + new Vector3(0, 2.5f, 0);
                    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    redPoints++;
                }

                if (Vector3.Distance(ball.transform.position, triggers.ElementAt(1).transform.position) < 3)
                {
                    ball.transform.position = GameMap.Position + new Vector3(0, 2.5f, 0);
                    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    bluePoints++;
                }

                if (ball.transform.position.y < GameMap.Position.y - 10f)
                {
                    ball.transform.position = GameMap.Position + new Vector3(0, 2.5f, 0);
                    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }

                yield return Timing.WaitForSeconds(0.3f);
                EventTime -= TimeSpan.FromSeconds(0.3f);
            }

            if (bluePoints > redPoints)
            {
                Extensions.Broadcast($"{translation.FootballBlueWins}", 10);
            }
            else if (redPoints > bluePoints)
            {
                Extensions.Broadcast($"{translation.FootballRedWins}", 10);
            }
            else
            {
                Extensions.Broadcast($"{translation.FootballDraw.Replace("{BluePnt}", $"{bluePoints}").Replace("{RedPnt}", $"{redPoints}")}", 3);
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
