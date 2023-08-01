using System;
using System.Collections.Generic;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Football.Features;
using Exiled.API.Enums;

namespace AutoEvent.Events.Football
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

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;

            OnEventStarted();
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 3, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Football.ogg", 5, true, Name);

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfCaptain, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;
            }

            Timing.RunCoroutine(OnEventRunning(), "glass_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
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

            while (bluePoints < 3 && redPoints < 3 && EventTime.TotalSeconds > 0 && Player.List.Count(r => r.IsNTF) > 0 && Player.List.Count(r => r.Role.Team == Team.ClassD) > 0)
            {
                var time = $"{EventTime.Minutes}:{EventTime.Minutes}";
                foreach (Player player in Player.List)
                {
                    var text = string.Empty;
                    if (Vector3.Distance(ball.transform.position, player.Position) < 2)
                    {
                        ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                        rig.AddForce(player.Transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
                    }

                    if (player.Role.Team == Team.FoundationForces)
                    {
                        text += AutoEvent.Singleton.Translation.FootballBlueTeam;
                    }
                    else
                    {
                        text += AutoEvent.Singleton.Translation.FootballRedTeam;
                    }

                    player.ClearBroadcasts();
                    player.Broadcast(1, text + AutoEvent.Singleton.Translation.FootballTimeLeft.Replace("{BluePnt}", $"{bluePoints}").Replace("{RedPnt}", $"{redPoints}").Replace("{eventTime}", time));
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

                yield return Timing.WaitForSeconds(0.1f);
                EventTime -= TimeSpan.FromSeconds(0.1f);
            }

            if (bluePoints > redPoints)
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.FootballBlueWins}", 10);
            }
            else if (redPoints > bluePoints)
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.FootballRedWins}", 10);
            }
            else
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.FootballDraw.Replace("{BluePnt}", $"{bluePoints}").Replace("{RedPnt}", $"{redPoints}")}", 3);
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
