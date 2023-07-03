using AutoEvent.Events.Versus.Features;
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

namespace AutoEvent.Events.Versus
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.VersusName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.VersusDescription + " [Beta]";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "versus";
        public SchematicObject GameMap { get; set; }
        public Player Scientist { get; set; }
        public Player ClassD { get; set; }
        public TimeSpan EventTime { get; set; }

        private bool isFreindlyFireEnabled;

        EventHandler _eventHandler;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Dying += _eventHandler.OnDying;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;

            OnEventStarted();
        }

        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnDying;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Scientist = null;
            ClassD = null;

            GameMap = Extensions.LoadMap("35Hp", new Vector3(10f, 1020f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Knife.ogg", 10, true, Name);
            
            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.Scientist, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;

                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = item;
                });
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

            var triggerScintist = GameMap.AttachedBlocks.Where(x => x.name == "TriggerScientist").First();
            var triggerClassD = GameMap.AttachedBlocks.Where(x => x.name == "TriggerClassD").First();

            while (Player.List.Count(r => r.Role == RoleTypeId.Scientist) > 0 && Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0)
            {
                foreach (Player player in Player.List)
                {
                    if (Scientist == null)
                    {
                        if (player.Role.Type == RoleTypeId.Scientist && Vector3.Distance(player.Position, triggerScintist.transform.position) <= 1f)
                        {
                            Scientist = player;
                            Scientist.Position = GameMap.Position + new Vector3(-11.351f, -3.424f, -7.284f);

                            if (ClassD != null && AutoEvent.Singleton.Config.VersusConfig.HealApponentWhenSomeoneEnterArea)
                            {
                                ClassD.Heal(100);
                            }
                        }
                    }

                    if (ClassD == null)
                    {
                        if (player.Role.Type == RoleTypeId.ClassD && Vector3.Distance(player.Position, triggerClassD.transform.position) <= 1f)
                        {
                            ClassD = player;
                            ClassD.Position = GameMap.Position + new Vector3(-20.0f, -3.424f, -7.284f);

                            if (Scientist != null && AutoEvent.Singleton.Config.VersusConfig.HealApponentWhenSomeoneEnterArea)
                            {
                                Scientist.Heal(100);
                            }
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
            AutoEvent.ActiveEvent = null;
        }
    }
}
