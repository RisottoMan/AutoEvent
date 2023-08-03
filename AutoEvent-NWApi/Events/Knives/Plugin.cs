using AutoEvent.Events.Knives.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Events.Knives
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.KnivesName;
        public override string Description { get; set; } = Translation.KnivesDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "35hp_2";
        public override string CommandName { get; set; } = "knife";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        private bool isFreindlyFireEnabled;

        EventHandler _eventHandler;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            OnEventStarted();
        }
        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            EventManager.UnregisterEvents(_eventHandler);
            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(5f, 1030f, -45f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Knife.ogg", 10, true, Name);

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.SetRole(RoleTypeId.NtfCaptain, RoleChangeReason.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    player.SetRole(RoleTypeId.ChaosRepressor, RoleChangeReason.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;

                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = item;
                });
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

            foreach(var wall in GameMap.AttachedBlocks.Where(x => x.name == "Wall"))
            {
                GameObject.Destroy(wall);
            }

            while (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > 0 && Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) > 0)
            {
                string mtfCount = Player.GetPlayers().Count(r => r.Team == Team.FoundationForces).ToString();
                string chaosCount = Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency).ToString();
                Extensions.Broadcast(Translation.KnivesCycle.
                    Replace("{name}", Name).
                    Replace("{mtfcount}", mtfCount).
                    Replace("{chaoscount}", chaosCount), 1);

                yield return Timing.WaitForSeconds(1f);
            }

            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.KnivesChaosWin.Replace("{name}", Name), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.KnivesMtfWin.Replace("{name}", Name), 10);
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
