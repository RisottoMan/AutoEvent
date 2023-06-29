using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace AutoEvent.Events.Pazzle
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Pazzle";
        public override string Description { get; set; } = "alpha testing";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "pazzle";
        public SchematicObject GameMap { get; set; }
        public Player Scientist { get; set; }
        public Player ClassD { get; set; }
        public TimeSpan EventTime { get; set; }

        private bool isFreindlyFireEnabled;

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
            Server.FriendlyFire = isFreindlyFireEnabled;

            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Scientist = null;
            ClassD = null;
            GameMap = Extensions.LoadMap("Pazzle", new Vector3(127.460f, 1016.707f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("Knife.ogg", 10, true, Name);

            var count = 0;
            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = GameMap.Position;
                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "pazzle_run");
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
                            if (ClassD != null && AutoEvent.Singleton.Config.VersusConfig.HealApponentWhenSomeoneEnterArea) ClassD.Heal(100);
                            Scientist.Position = GameMap.Position + new Vector3(-11.351f, -3.424f, -7.284f);
                        }
                    }
                    if (ClassD == null)
                    {
                        if (Vector3.Distance(player.Position, GameMap.Position + new Vector3(-21.4718f, -3.871f, -7.284f)) <= 0.5f)
                        {
                            ClassD = player;
                            if (Scientist != null && AutoEvent.Singleton.Config.VersusConfig.HealApponentWhenSomeoneEnterArea) ClassD.Heal(100);
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
