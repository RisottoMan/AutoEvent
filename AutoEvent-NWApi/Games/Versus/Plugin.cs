using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.API.Schematic.Objects;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Hints;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Versus
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.VersusTranslate.VersusName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.VersusTranslate.VersusDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "versus";
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "35Hp", Position = new Vector3(6f, 1015f, -5f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Knife.ogg", Volume = 10, Loop = true };
        private EventHandler EventHandler { get; set; }
        private VersusTranslate Translation { get; set; }
        public Player Scientist { get; set; }
        public Player ClassD { get; set; }
        private List<GameObject> _triggers;
        private List<GameObject> _teleports;
        private TimeSpan _countdown;

        protected override void RegisterEvents()
        {
            Translation = new VersusTranslate();

            EventHandler = new EventHandler(this);
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropItem -= EventHandler.OnDropItem;
            Players.DropAmmo -= EventHandler.OnDropAmmo;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            Scientist = null;
            ClassD = null;
            
            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    Extensions.SetRole(player, RoleTypeId.Scientist, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }
                count++;

                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.2f, () =>
                {
                    player.CurrentItem = item;
                });
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            _triggers = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Trigger").ToList();
            _teleports = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Teleport").ToList();
            _countdown = new TimeSpan(0, 0, 15);
        }

        protected override bool IsRoundDone()
        {
            // At least 1 player on scientists &&
            // At least 1 player on dbois
            return !(Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) > 0 &&
                   Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0);
        }

        protected override void ProcessFrame()
        {
            foreach (Player player in Player.GetPlayers())
                {
                    if (Scientist == null)
                    {
                        if (player.Role == RoleTypeId.Scientist &&
                            (Vector3.Distance(player.Position, _triggers.ElementAt(0).transform.position) <= 1f || _countdown.TotalSeconds == 0))
                        {
                            Scientist = player;
                            Scientist.Position = _teleports.ElementAt(0).transform.position;
                            if (ClassD != null) 
                                ClassD.Heal(100);

                            _countdown = new TimeSpan(0, 0, 15);
                        }
                    }

                    if (ClassD == null)
                    {
                        if (player.Role == RoleTypeId.ClassD &&
                            (Vector3.Distance(player.Position, _triggers.ElementAt(1).transform.position) <= 1f || _countdown.TotalSeconds == 0))
                        {
                            ClassD = player;
                            ClassD.Position = _teleports.ElementAt(1).transform.position;
                            if (Scientist != null) 
                                Scientist.Heal(100);

                            _countdown = new TimeSpan(0, 0, 15);
                        }
                    }
                }

                if (ClassD == null && Scientist == null)
                {
                    Extensions.Broadcast(Translation.VersusPlayersNull.
                        Replace("{name}", Name).
                        Replace("{remain}", $"{_countdown.TotalSeconds}"), 1);
                }
                else if (ClassD == null)
                {
                    Extensions.Broadcast(Translation.VersusClassDNull.
                        Replace("{name}", Name).
                        Replace("{scientist}", Scientist.Nickname).
                        Replace("{remain}", $"{_countdown.TotalSeconds}"), 1);
                }
                else if (Scientist == null)
                {
                    Extensions.Broadcast(Translation.VersusScientistNull.
                        Replace("{name}", Name).
                        Replace("{classd}", ClassD.Nickname).
                        Replace("{remain}", $"{_countdown.TotalSeconds}"), 1);
                }
                else
                {
                    Extensions.Broadcast(Translation.VersusPlayersDuel.
                        Replace("{name}", Name).
                        Replace("{scientist}", Scientist.Nickname).
                        Replace("{classd}", ClassD.Nickname), 1);
                }

                _countdown = _countdown.Subtract(new TimeSpan(0,0, 1));
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) == 0)
            {
                Extensions.Broadcast(Translation.VersusClassDWin.Replace("{name}", Name), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(Translation.VersusScientistWin.Replace("{name}", Name), 10);
            }
        }
    }
}
