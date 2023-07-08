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
using Exiled.Events.Commands.Reload;

namespace AutoEvent.Events.Football
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.FootballName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.FootballDescription;
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "football";
        public SchematicObject GameMap { get; set; }
        public int BluePoints { get; set; } = 0;
        public int RedPoints { get; set; } = 0;
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
            GameMap = Extensions.LoadMap("Football", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            EventTime = new TimeSpan(0, 3, 0);
            BluePoints = 0;
            RedPoints = 0;

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }

                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "glass_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var ball = GameMap.AttachedBlocks.First(x => x.name == "Ball");
            ball.AddComponent<BallComponent>();

            var triggerBlue = GameMap.AttachedBlocks.First(x => x.name == "TriggerBlue");
            var triggerOrange = GameMap.AttachedBlocks.First(x => x.name == "TriggerOrange");

            while (BluePoints < 2 && RedPoints < 2 && EventTime.TotalSeconds > 0 && Player.List.Count(r => r.IsAlive) > 1) // всё-равно переработать эту хуйню
            {
                var text = string.Empty;
                foreach (Player player in Player.List)
                {
                    if (Vector3.Distance(ball.transform.position, player.Position) < 2)
                    {
                        ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                        rig.AddForce(player.Transform.forward + new Vector3(0, 0.5f, 0), ForceMode.Impulse);
                    }

                    if (player.Role.Type == RoleTypeId.NtfCaptain)
                    {
                        text += AutoEvent.Singleton.Translation.FootballBlueTeam;
                    }
                    else
                    {
                        text += AutoEvent.Singleton.Translation.FootballRedTeam;
                    }

                    player.ClearBroadcasts();
                    player.Broadcast(1, text + AutoEvent.Singleton.Translation.FootballTimeLeft.Replace("{BluePnt}", $"{BluePoints}").Replace("{RedPnt}", $"{RedPoints}").Replace("{eventTime}", $"{EventTime.Minutes}:{EventTime.Seconds}"));
                }

                if (Vector3.Distance(ball.transform.position, triggerBlue.transform.position) < 3)
                {
                    ball.transform.position = GameMap.Position + new Vector3(0, 2.5f, 0);
                    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    RedPoints++;
                }

                if (Vector3.Distance(ball.transform.position, triggerOrange.transform.position) < 3)
                {
                    ball.transform.position = GameMap.Position + new Vector3(0, 2.5f, 0);
                    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    BluePoints++;
                }

                yield return Timing.WaitForSeconds(0.1f);
                EventTime -= TimeSpan.FromSeconds(0.1f);
            }

            if (BluePoints > RedPoints)
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.FootballBlueWins}", 10);
            }
            else if (RedPoints > BluePoints)
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.FootballRedWins}", 10);
            }
            else
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.FootballTie.Replace("{BluePnt}", $"{BluePoints}").Replace("{RedPnt}", $"{RedPoints}")}", 3);
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
