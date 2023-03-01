using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Events
{
    internal class InfectionEvent : IEvent
    {
        public string Name => AutoEvent.Singleton.Translation.ZombieName;
        public string Description => AutoEvent.Singleton.Translation.ZombieDescription;
        public string Color => "FF4242";
        public string CommandName => "zombie";
        public static SchematicObject GameMap { get; set; }
        public static TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += InfectionHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died += InfectionHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting += InfectionHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam += InfectionHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= InfectionHandler.OnJoin;
            Exiled.Events.Handlers.Player.Died -= InfectionHandler.OnDead;
            Exiled.Events.Handlers.Player.Hurting -= InfectionHandler.OnDamage;
            Exiled.Events.Handlers.Server.RespawningTeam -= InfectionHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("Zombie", new Vector3(115.5f, 1030f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            switch(Random.Range(0, 1))
            {
                case 0: Extensions.PlayAudio("Zombie.ogg", 15, true, Name); break;
                case 1: Extensions.PlayAudio("Zombie2.ogg", 15, true, Name); break;
            }
            foreach(Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                player.Position = GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
                player.ClearInventory();
            }
            Timing.RunCoroutine(OnEventRunning(), "zombie_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            // Counting down
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast(trans.ZombieBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }
            // Spawn zombie
            Player.List.ToList().RandomItem().Role.Set(RoleTypeId.Scp0492);
            // Until there is one player left, the game will not end
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
            // If there is only one person left, then the countdown will start
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
        }
    }
}
