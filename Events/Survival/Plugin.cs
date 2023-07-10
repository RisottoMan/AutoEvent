using AutoEvent.Interfaces;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Survival
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.SurvivalName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.SurvivalDescription;
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "zombie2";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        private bool isFriendlyFireEnabled;
        EventHandler _eventHandler;
        public Player firstZombie;
        public override void OnStart()
        {
            isFriendlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

            OnEventStarted();

            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned += _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon += _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnDamage;
            Exiled.Events.Handlers.Player.Died += _eventHandler.OnDead;
        }
        public override void OnStop()
        {
            Server.FriendlyFire = isFriendlyFireEnabled;

            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnDamage;
            Exiled.Events.Handlers.Player.Died -= _eventHandler.OnDead;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 5, 0);

            GameMap = Extensions.LoadMap("Survival", new Vector3(15f, 1030f, -43.68f), Quaternion.identity, Vector3.one);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.NtfSergeant, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
                player.AddAhp(100, 100, 0, 0, 0, true);

                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = player.Items.ElementAt(1);
                });
            }

            Timing.RunCoroutine(OnEventRunning(), "survival_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            Extensions.PlayAudio("Survival.ogg", 10, false, Name);
            for (float _time = 20; _time > 0; _time--)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.SurvivalBeforeInfection.Replace("%name%", Name).Replace("%time%", $"{_time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Extensions.StopAudio();
            Timing.CallDelayed(0.1f, () =>
            {
                Extensions.PlayAudio("Zombie2.ogg", 10, false, Name);
            });

            for (int i = 0; i <= Player.List.Count() / 10; i++)
            {
                var player = Player.List.Where(r => r.IsHuman).ToList().RandomItem();
                player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                player.EnableEffect<Disabled>();
                player.EnableEffect<Scp1853>();
                player.Health = 10000;

                if (Player.List.Count(r => r.IsScp) == 1)
                {
                    firstZombie = player;
                }
            }

            var teleport = GameMap.AttachedBlocks.First(x => x.name == "Teleport");
            var teleport1 = GameMap.AttachedBlocks.First(x => x.name == "Teleport1");

            while (Player.List.Count(r => r.IsHuman) > 0 && Player.List.Count(r => r.IsScp) > 0 && EventTime.TotalSeconds > 0)
            {
                var text = AutoEvent.Singleton.Translation.SurvivalAfterInfection;
                text = text.Replace("%name%", Name);
                text = text.Replace("%humanCount%", Player.List.Count(r => r.IsHuman).ToString());
                text = text.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}");

                foreach (var player in Player.List)
                {
                    player.ClearBroadcasts();
                    player.Broadcast(1, text);

                    if (Vector3.Distance(player.Position, teleport.transform.position) < 1)
                    {
                        player.Position = teleport1.transform.position;
                    }
                }

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.IsHuman) == 0)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.SurvivalZombieWin, 10);

                Extensions.StopAudio();
                Timing.CallDelayed(0.1f, () =>
                {
                    Extensions.PlayAudio("ZombieWin.ogg", 10, false, Name);
                });
            }
            else if (Player.List.Count(r => r.IsScp) == 0)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.SurvivalHumanWin, 10);

                Extensions.StopAudio();
                Timing.CallDelayed(0.1f, () =>
                {
                    Extensions.PlayAudio("HumanWin.ogg", 10, false, Name);
                });
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.SurvivalHumanWinTime, 10);

                Extensions.StopAudio();
                Timing.CallDelayed(0.1f, () =>
                {
                    Extensions.PlayAudio("HumanWin.ogg", 10, false, Name);
                });
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
