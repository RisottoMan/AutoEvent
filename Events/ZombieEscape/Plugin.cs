using AutoEvent.Events.ZombieEscape.Features;
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

namespace AutoEvent.Events.ZombieEscape
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.ZombieEscapeName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.ZombieEscapeDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "zm_osprey";
        public override string CommandName { get; set; } = "zombie3";
        public SchematicObject GameMap { get; set; }
        public SchematicObject Boat { get; set; }
        public SchematicObject Heli { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;
        private bool isFriendlyFireEnabled;

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

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 5, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(-15f, 1020f, -80f), Quaternion.identity, Vector3.one);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.NtfSergeant, SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
                player.AddAhp(100, 100, 0, 0, 0, true);

                player.AddItem(RandomClass.GetRandomGun());
                player.AddItem(ItemType.GunCOM18);
                player.AddItem(ItemType.ArmorCombat);

                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = player.Items.ElementAt(0);
                });
            }

            Timing.RunCoroutine(OnEventRunning(), "zmescape_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            Extensions.PlayAudio("Survival.ogg", 10, false, Name);

            for (float _time = 20; _time > 0; _time--)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.ZombieEscapeBeforeStart.Replace("%name%", Name).Replace("%time%", $"{_time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Extensions.StopAudio();
            Timing.CallDelayed(0.1f, () =>
            {
                Extensions.PlayAudio("Zombie2.ogg", 7, false, Name);
            });

            for (int i = 0; i <= Player.List.Count() / 10; i++)
            {
                var player = Player.List.Where(r => r.IsHuman).ToList().RandomItem();
                player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                player.EnableEffect<Disabled>();
                player.EnableEffect<Scp1853>();
                player.Health = 10000;
            }

            GameObject button = new GameObject();
            GameObject button1 = new GameObject();
            GameObject button2 = new GameObject();
            GameObject wall = new GameObject();
            Vector3 finish = new Vector3();

            foreach (var gameObject in GameMap.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Button": { button = gameObject; } break;
                    case "Button1": { button1 = gameObject; } break;
                    case "Button2": { button2 = gameObject; } break;
                    case "Lava": { gameObject.AddComponent<LavaComponent>(); } break;
                    case "Wall": { wall = gameObject; } break;
                    case "Finish": { finish = gameObject.transform.position; } break;
                }
            }

            while (Player.List.Count(r => r.IsHuman) > 0 && Player.List.Count(r => r.IsScp) > 0 && EventTime.TotalSeconds > 0)
            {
                foreach(Player player in Player.List)
                {
                    /*
                    if (Vector3.Distance(player.Position, button.transform.position) < 3)
                    {
                        button.transform.position += Vector3.down * 7;
                        Boat = Extensions.LoadMap("Boat_Zombie", GameMap.Position, Quaternion.identity, Vector3.one);
                    }
                    */
                    if (Vector3.Distance(player.Position, button1.transform.position) < 3)
                    {
                        button1.transform.position += Vector3.down * 5;
                        wall.AddComponent<WallComponent>();
                    }

                    if (Vector3.Distance(player.Position, button2.transform.position) < 3)
                    {
                        button2.transform.position += Vector3.down * 5;
                        EventTime = new TimeSpan(0, 1, 5);
                        Heli = Extensions.LoadMap("Helicopter_Zombie", GameMap.Position, Quaternion.identity, Vector3.one);
                    }

                    string text = AutoEvent.Singleton.Translation.ZombieEscapeHelicopter.Replace("%name%", Name).Replace("%count%", $"{Player.List.Count(r => r.IsHuman)}");
                    player.ClearBroadcasts();
                    player.Broadcast(1, text);
                }

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            foreach(Player player in Player.List)
            {
                player.EnableEffect<Flashed>(1);

                if (Heli != null)
                {
                    if (Vector3.Distance(player.Position, finish) > 5)
                    {
                        player.Hurt(15000f, AutoEvent.Singleton.Translation.ZombieEscapeDied);
                    }
                }
            }

            if (Player.List.Count(r => r.IsHuman) == 0)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.ZombieEscapeZombieWin, 10);
                Extensions.StopAudio();
                Timing.CallDelayed(0.1f, () =>
                {
                    Extensions.PlayAudio("ZombieWin.ogg", 7, false, Name);
                });
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.ZombieEscapeHumanWin, 10);
                Extensions.StopAudio();
                Timing.CallDelayed(0.1f, () =>
                {
                    Extensions.PlayAudio("HumanWin.ogg", 7, false, Name);
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
            if (Boat != null) Extensions.UnLoadMap(Boat);
            if (Heli != null) Extensions.UnLoadMap(Heli);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
