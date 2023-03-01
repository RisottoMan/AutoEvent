using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;
using Exiled.API.Enums;
using Random = UnityEngine.Random;

namespace AutoEvent.Events
{
    internal class VersusEvent : IEvent
    {
        public string Name => AutoEvent.Singleton.Translation.VersusName;
        public string Description => AutoEvent.Singleton.Translation.VersusDescription;
        public string Color => "FFFF00";
        public string CommandName => "versus";
        public SchematicObject GameMap { get; set; }
        public static Player Scientist { get; set; }
        public static Player ClassD { get; set; }
        public TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += VersusHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem += VersusHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam += VersusHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Died += VersusHandler.OnDead;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= VersusHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem -= VersusHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam -= VersusHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Died -= VersusHandler.OnDead;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Scientist = null;
            ClassD = null;
            GameMap = Extensions.LoadMap("35Hp", new Vector3(127.460f, 1016.707f, -43.68f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            Extensions.PlayAudio("Knife.ogg", 10, true, Name);

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.Scientist);
                    player.Position = GameMap.Position + new Vector3(-2.91f, -3.2f, Random.Range(-1, -13));
                }
                else
                {
                    player.Role.Set(RoleTypeId.ClassD);
                    player.Position = GameMap.Position + new Vector3(-28.65f, -3.2f, Random.Range(-1, -13));
                }
                player.ResetInventory(new List<ItemType> { ItemType.Jailbird });
                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "versus_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
            while (Player.List.Count(r => r.Role == RoleTypeId.Scientist) > 0 && Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0)
            {
                foreach (Player player in Player.List)
                {
                    if (Scientist == null)
                    {
                        if (Vector3.Distance(player.Position, GameMap.Position + new Vector3(-10.233f, -3.871f, -7.284f)) <= 0.5f)
                        {
                            Scientist = player;
                            Scientist.Position = GameMap.Position + new Vector3(-11.351f, -3.424f, -7.284f);
                        }
                    }
                    if (ClassD == null)
                    {
                        if (Vector3.Distance(player.Position, GameMap.Position + new Vector3(-21.4718f, -3.871f, -7.284f)) <= 0.5f)
                        {
                            ClassD = player;
                            ClassD.Position = GameMap.Position + new Vector3(-20.0f, -3.424f, -7.284f);
                        }
                    }
                }
                if (ClassD == null && Scientist == null)
                {
                    Extensions.Broadcast(trans.VersusPlayersNull.Replace("{name}", Name), 1);
                }
                else if (ClassD == null)
                {
                    Extensions.Broadcast(trans.VersusClassDNull.Replace("{name}", Name).Replace("{scientist}", Scientist.Nickname), 1);
                }
                else if (Scientist == null)
                {
                    Extensions.Broadcast(trans.VersusScientistNull.Replace("{name}", Name).Replace("{classd}", ClassD.Nickname), 1);
                }
                else
                {
                    Extensions.Broadcast(trans.VersusPlayersDuel.Replace("{name}", Name).Replace("{scientist}", Scientist.Nickname).Replace("{classd}", ClassD.Nickname), 1);
                }
                yield return Timing.WaitForSeconds(0.3f);
            }
            if (Player.List.Count(r => r.Role == RoleTypeId.Scientist) == 0)
            {
                Extensions.Broadcast(trans.VersusClassDWin.Replace("{name}", Name), 10);
            }
            else if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(trans.VersusScientistWin.Replace("{name}", Name), 10);
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
        }
    }
}
