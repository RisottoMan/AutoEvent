using AutoEvent.Events.Lava.Features;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Lava
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.LavaName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.LavaDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Lava";
        public override string CommandName { get; set; } = "lava";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }

        EventHandler _eventHandler;
        private bool isFreindlyFireEnabled;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = true;

            OnEventStarted();

            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned += _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon += _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurt;
        }
        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurt;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            GameMap = Extensions.LoadMap(MapName, new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Lava.ogg", 7, false, Name);

            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "lava_time");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.LavaBeforeStart.Replace("%time%", $"{time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            GameObject lava = GameMap.AttachedBlocks.First(x => x.name == "LavaObject");
            lava.AddComponent<LavaComponent>();

            while (Player.List.Count(r => r.IsAlive) > 1)
            {
                string text = string.Empty;
                if (EventTime.TotalSeconds % 2 == 0)
                {
                    text = "<size=90><color=red><b>《 ! 》</b></color></size>\n";
                }
                else
                {
                    text = "<size=90><color=red><b>!</b></color></size>\n";
                }

                Extensions.Broadcast(text + AutoEvent.Singleton.Translation.LavaCycle.Replace("%count%", $"{Player.List.Count(r => r.IsAlive)}"), 1);
                lava.transform.position += new Vector3(0, 0.08f, 0);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.LavaWin.Replace("%winner%", Player.List.First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.LavaAllDead, 10);
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
