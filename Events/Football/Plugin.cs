using System;
using System.Collections.Generic;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;
using Component = AutoEvent.Events.Football.Features.Component;
using AutoEvent.Events.Football.Features;
using Exiled.Events.Commands.Reload;

namespace AutoEvent.Events.Football
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Футбольчик [Testing]";
        public override string Description { get; set; } = "Режим Футбол, в котором надо забить 3 гола противоположной команде. [Testing]";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "football";
        public SchematicObject GameMap { get; set; }
        public GameObject Ball { get; set; }
        public GameObject TriggerBlue { get; set; }
        public GameObject TriggerOrange { get; set; }
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

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            GameMap = Extensions.LoadMap("Football", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            EventTime = new TimeSpan(0, 3, 0);
            BluePoints = 0;
            RedPoints = 0;

            Ball = GameMap.AttachedBlocks.First(x => x.name == "Ball");
            Ball.AddComponent<Component>();
            TriggerBlue = GameMap.AttachedBlocks.First(x => x.name == "TriggerBlue");
            TriggerOrange = GameMap.AttachedBlocks.First(x => x.name == "TriggerOrange");

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

                // Need rework physic
                //var collider = player.GameObject.AddComponent<BoxCollider>();
                //collider.size = new Vector3(0, 0, 0);

                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "glass_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            while (BluePoints < 2 && RedPoints < 2 && EventTime.TotalSeconds > 0 && Player.List.Count(r => r.IsAlive) > 1)
            {
                foreach (Player player in Player.List)
                {
                    var text = string.Empty;
                    if (player.Role.Type == RoleTypeId.NtfCaptain)
                    {
                        text += "<color=blue>Вы играете за Синюю Команду</color>\n";
                    }
                    else
                    {
                        text += "<color=red>Вы играете за Красную Команду</color>\n";
                    }

                    if (Vector3.Distance(Ball.transform.position, player.Position) < 2)
                    {
                        Ball.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rig);
                        rig.AddForce(player.Transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
                    }

                    Extensions.Broadcast(text + $"<color=blue>{BluePoints}</color> VS <color=red>{RedPoints}</color>\n" +
                        $"Время до конца: {EventTime.Minutes}:{EventTime.Seconds}", 1);
                }

                if (Vector3.Distance(Ball.transform.position, TriggerBlue.transform.position) < 3)
                {
                    Ball.transform.position = GameMap.Position + new Vector3(0, 2.5f, 0);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    RedPoints++;
                }

                if (Vector3.Distance(Ball.transform.position, TriggerOrange.transform.position) < 3)
                {
                    Ball.transform.position = GameMap.Position + new Vector3(0, 2.5f, 0);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    BluePoints++;
                }

                yield return Timing.WaitForSeconds(0.1f);
                EventTime -= TimeSpan.FromSeconds(0.1f);
            }

            if (BluePoints > RedPoints)
            {
                Extensions.Broadcast($"<color=blue>ПОБЕДА СИНИХ!</color>", 10);
            }
            else if (RedPoints > BluePoints)
            {
                Extensions.Broadcast($"<color=red>ПОБЕДА КРАСНЫХ!</color>", 10);
            }
            else
            {
                Extensions.Broadcast($"<color=#808080>Ничья</color>\n<color=blue>{BluePoints}</color> VS <color=red>{RedPoints}</color>", 10);
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            GameObject.Destroy(Ball);
            GameObject.Destroy(TriggerBlue);
            GameObject.Destroy(TriggerOrange);
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
