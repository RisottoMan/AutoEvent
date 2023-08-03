using AutoEvent.Events.Versus.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Events.Versus
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.VersusName;
        public override string Description { get; set; } = Translation.VersusDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "35Hp";
        public override string CommandName { get; set; } = "versus";
        public SchematicObject GameMap { get; set; }
        public Player Scientist { get; set; }
        public Player ClassD { get; set; }

        private bool isFreindlyFireEnabled;

        EventHandler _eventHandler;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

            _eventHandler = new EventHandler(this);
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
            Scientist = null;
            ClassD = null;

            GameMap = Extensions.LoadMap(MapName, new Vector3(6f, 1015f, -5f), Quaternion.identity, Vector3.one);
            Extensions.PlayAudio("Knife.ogg", 10, true, Name);
            
            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.SetRole(RoleTypeId.Scientist, RoleChangeReason.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    player.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;

                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.2f, () =>
                {
                    player.CurrentItem = item;
                });
            }

            Timing.RunCoroutine(OnEventRunning(), "versus_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            var triggers = GameMap.AttachedBlocks.Where(x => x.name == "Trigger");
            var teleports = GameMap.AttachedBlocks.Where(x => x.name == "Teleport");
            var remain = new TimeSpan(0, 0, 15);

            while (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) > 0 && Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0)
            {
                foreach (Player player in Player.GetPlayers())
                {
                    if (Scientist == null)
                    {
                        if (player.Role == RoleTypeId.Scientist && (Vector3.Distance(player.Position, triggers.ElementAt(0).transform.position) <= 1f || remain.TotalSeconds == 0))
                        {
                            Scientist = player;
                            Scientist.Position = teleports.ElementAt(0).transform.position;
                            if (ClassD != null) ClassD.Heal(100);

                            remain = new TimeSpan(0, 0, 15);
                        }
                    }

                    if (ClassD == null)
                    {
                        if (player.Role == RoleTypeId.ClassD && (Vector3.Distance(player.Position, triggers.ElementAt(1).transform.position) <= 1f || remain.TotalSeconds == 0))
                        {
                            ClassD = player;
                            ClassD.Position = teleports.ElementAt(1).transform.position;
                            if (Scientist != null) Scientist.Heal(100);

                            remain = new TimeSpan(0, 0, 15);
                        }
                    }
                }

                if (ClassD == null && Scientist == null)
                {
                    Extensions.Broadcast(Translation.VersusPlayersNull.
                        Replace("{name}", Name).
                        Replace("{remain}", $"{remain.TotalSeconds}"), 1);
                }
                else if (ClassD == null)
                {
                    Extensions.Broadcast(Translation.VersusClassDNull.
                        Replace("{name}", Name).
                        Replace("{scientist}", Scientist.Nickname).
                        Replace("{remain}", $"{remain.TotalSeconds}"), 1);
                }
                else if (Scientist == null)
                {
                    Extensions.Broadcast(Translation.VersusScientistNull.
                        Replace("{name}", Name).
                        Replace("{classd}", ClassD.Nickname).
                        Replace("{remain}", $"{remain.TotalSeconds}"), 1);
                }
                else
                {
                    Extensions.Broadcast(Translation.VersusPlayersDuel.
                        Replace("{name}", Name).
                        Replace("{scientist}", Scientist.Nickname).
                        Replace("{classd}", ClassD.Nickname), 1);
                }

                remain -= TimeSpan.FromSeconds(1f);
                yield return Timing.WaitForSeconds(1f);
            }

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) == 0)
            {
                Extensions.Broadcast(Translation.VersusClassDWin.Replace("{name}", Name), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(Translation.VersusScientistWin.Replace("{name}", Name), 10);
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
