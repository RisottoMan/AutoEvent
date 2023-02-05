using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Events
{
    internal class Knives : IEvent
    {
        public string Name => "Ножики";
        public string Description => "Игроки на ножах против друг друга на карте 35hp из cs 1.6";
        public string Color => "FFFF00";
        public string CommandName => "knife";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += KnivesHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem += KnivesHandler.OnDropItem;
            Exiled.Events.Handlers.Server.RespawningTeam += KnivesHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= KnivesHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem -= KnivesHandler.OnDropItem;
            Exiled.Events.Handlers.Server.RespawningTeam -= KnivesHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            //Plugin.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("35hp_2", new Vector3(115.5f, 1030f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            Extensions.PlayAudio("Knife.ogg", 10, true, "Ножики");
            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfCaptain);
                    player.Position = GameMap.Position + new Vector3(Random.Range(12, 19), 5, Random.Range(-9, 9));
                }
                else
                {
                    player.Role.Set(RoleTypeId.ChaosRepressor);
                    player.Position = GameMap.Position + new Vector3(Random.Range(-18, -11), 5, Random.Range(-9, 9));
                }
                player.ResetInventory(new List<ItemType> { ItemType.Jailbird, ItemType.Jailbird });
                player.EnableEffect<CustomPlayerEffects.Ensnared>(10);
                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "knives_run");

        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            foreach(Player player in Player.List)
            {
                player.DisableEffect<CustomPlayerEffects.Ensnared>();
            }

            // cycle
            while (Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0 && Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) > 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>НОЖИКИ</i></b></color>\n" +
                $"<color=yellow><color=blue>{Player.List.Count(r => r.Role.Team == Team.FoundationForces)} МОГ</color> <color=red>VS</color> <color=green>{Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency)} Хаос</color></color>", 1);

                yield return Timing.WaitForSeconds(1f);
            }
            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>НОЖИКИ</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИТЕЛИ: <color=green>ХАОС</color></color>", 10);
            }
            else if (Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>НОЖИКИ</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИТЕЛИ: <color=blue>МОГ</color></color>", 10);
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
