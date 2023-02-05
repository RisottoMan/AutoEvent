using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace AutoEvent.Events
{
    internal class VersusEvent : IEvent
    {
        public string Name => "Петушиные Бои";
        public string Description => "Дуель игроков на карте 35hp из cs 1.6";
        public string Color => "FFFF00";
        public string CommandName => "versus";
        public SchematicObject GameMap { get; set; }
        public Player Scientist { get; set; }
        public Player ClassD { get; set; }
        public TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += VersusHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem += VersusHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam += VersusHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= VersusHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem -= VersusHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam -= VersusHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("35Hp", new Vector3(115.5f, 1030f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            Extensions.PlayAudio("Knife.ogg", 5, true, "Петушиные Бои");

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.Scientist);
                    player.Position = GameMap.Position + new Vector3(-5.445f, -3.424f, -7.284f);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ClassD);
                    player.Position = GameMap.Position + new Vector3(-26.788f, -3.424f, -7.488f);
                }
                player.ResetInventory(new List<ItemType> { ItemType.Jailbird });
                count++;
            }
            //Timing.RunCoroutine(OnEventRunning(), "versus_run");

        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.List.Count(r => r.Role.Team == Team.Scientists) > 0 && Player.List.Count(r => r.Role.Team == Team.ClassD) > 0)
            {
                foreach(Player player in Player.List)
                {
                    if (Vector3.Distance(player.Position, new Vector3(-10.233f, -3.871f, -7.284f)) < 0.5f && (Scientist == null || Scientist.IsDead))
                    {
                        Scientist = player;
                        player.Position = GameMap.Position + new Vector3(-11.351f, -3.424f, -7.284f);
                        player.AddItem(ItemType.Jailbird);
                    }
                    if (Vector3.Distance(player.Position, new Vector3(-21.4718f, -3.871f, -7.284f)) < 0.5f && (ClassD == null || ClassD.IsDead))
                    {
                        ClassD = player;
                        player.Position = GameMap.Position + new Vector3(-20.514f, -3.424f, -7.284f);
                        player.AddItem(ItemType.Jailbird);
                    }
                }
                if (ClassD.IsDead || ClassD == null)
                {
                    Extensions.Broadcast($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                    $"В живых остался игрок <color=yellow>{Scientist.Nickname}</color>", 1);
                }
                else if (Scientist.IsDead || Scientist == null)
                {
                    Extensions.Broadcast($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                    $"В живых остался игрок <color=orange>{ClassD.Nickname}</color>", 1);
                }
                else if ((ClassD.IsDead || ClassD == null) && (Scientist.IsDead || Scientist == null))
                {
                    Extensions.Broadcast($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                    $"Зайдите внутрь арены, чтобы подраться друг с другом!", 1);
                }
                else
                {
                    Extensions.Broadcast($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                    $"<color=yellow><color=yellow>{Scientist.Nickname}</color> <color=red>VS</color> <color=orange>{ClassD.Nickname}</color></color>", 1);
                }
                yield return Timing.WaitForSeconds(1f);
            }
            if (Player.List.Count(r => r.Role.Team == Team.Scientists) == 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИТЕЛИ: <color=red>Д КЛАСС</color></color>", 10);
            }
            else if (Player.List.Count(r => r.Role.Team == Team.ClassD) == 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Петушиные Бои</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИТЕЛИ: <color=red>УЧЕНЫЕ</color></color>", 10);
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
