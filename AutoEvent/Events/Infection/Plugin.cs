using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Infection
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.ZombieName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.ZombieDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = AutoEvent.Singleton.Config.InfectionConfig.ListOfMap.RandomItem();
        public override string CommandName { get; set; } = "zombie";
        public static SchematicObject GameMap { get; set; }
        public static TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died += _eventHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;

            OnEventStarted();
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died -= _eventHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
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

            float scale = 1;
            switch(Player.List.Count())
            {
                case int n when (n > 15 && n <= 20): scale = 1.1f; break;
                case int n when (n > 20 && n <= 25): scale = 1.2f; break;
                case int n when (n > 25 && n <= 30): scale = 1.3f; break;
                case int n when (n > 30 && n <= 35): scale = 1.4f; break;
                case int n when (n > 35): scale = 1.5f; break;
            }

            GameMap = Extensions.LoadMap(MapName, new Vector3(115.5f, 1030f, -43.5f), Quaternion.identity, new Vector3(1, 1, 1) * scale);
            Extensions.PlayAudio(AutoEvent.Singleton.Config.InfectionConfig.ListOfMusic.RandomItem(), 7, true, Name);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "zombie_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;

            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast(trans.ZombieBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Player.List.ToList().RandomItem().Role.Set(RoleTypeId.Scp0492);

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 1)
            {
                var count = Player.List.Count(r => r.Role == RoleTypeId.ClassD);
                var time = $"{EventTime.Minutes}:{EventTime.Seconds}";

                Extensions.Broadcast(trans.ZombieCycle.Replace("{name}", Name).Replace("{count}", count.ToString()).Replace("{time}", time), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            Timing.RunCoroutine(DopTime(), "EventBeginning");
            yield break;
        }

        public IEnumerator<float> DopTime()
        {
            var trans = AutoEvent.Singleton.Translation;
            var time = $"{EventTime.Minutes}:{EventTime.Seconds}";

            for (int extratime = 30; extratime > 0; extratime--)
            {
                if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0) break;
                Extensions.Broadcast(trans.ZombieExtraTime.Replace("{extratime}", extratime.ToString()).Replace("{time}", time), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(trans.ZombieWin.Replace("{time}", time), 10);
            }
            else
            {
                Extensions.Broadcast(trans.ZombieLose.Replace("{time}", time), 10);
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

            MapName = AutoEvent.Singleton.Config.InfectionConfig.ListOfMap.RandomItem();
            AutoEvent.ActiveEvent = null;
        }
    }
}
