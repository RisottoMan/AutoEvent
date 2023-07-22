using AutoEvent.Events.Line.Features;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.Commands.Reload;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.Configs;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Valleyball
{
    internal class Plugin : IEvent
    {
        public string Name { get; set; } = "Волейбол [ALPHA]";
        public string Description { get; set; } = "Нужно забить гол противоположной команде";
        public string Color { get; set; } = "ffda36";
        public string CommandName { get; set; } = "valleyball";

        private EventHandler _eventHandler;
        private SchematicObject GameMap;
        private Vector3 SpawnPosition;
        private TimeSpan EventTime;
        private TimeSpan RoundTime;
        private int Rounds;
        private int Dcounts;
        private int Scounts;

        public void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;

            OnEventStarted();
        }

        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;

            Timing.CallDelayed(10f, () => EventEnd());
        }

        private void OnEventStarted()
        {
            GameMap = Extensions.LoadMap(CommandName, new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            SpawnPosition = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            EventTime = TimeSpan.Zero;
            RoundTime = TimeSpan.FromMinutes(2f);

            Player.List.ToList().ForEach(pl =>
            {
                pl.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.AssignInventory);
                pl.Position = SpawnPosition;
            });

            Timing.RunCoroutine(OnEventRunning(), "valleyball_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;

            for (int time = 10; time > 0; time-- )
            {
                Extensions.Broadcast(translation.TimerBeforeStart.Replace("%time%", $"{time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) >= 1 && Rounds <= 3 || Player.List.Count(r => r.Role == RoleTypeId.Scientist) >= 1 && Rounds <= 3) // change >= to >
            {
                Extensions.Broadcast(translation.ValleyballBroadcast.Replace("%color%", Color).Replace("%name%", Name).Replace("%count%", $"{Dcounts} | {Scounts}").Replace("%time%", $"{RoundTime.Minutes}:{RoundTime.Seconds}"), 1);

                RoundTime -= TimeSpan.FromSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
                yield return Timing.WaitForSeconds(1f);
            }

            string broadcast;

            if (Dcounts > Scounts) broadcast = translation.ValleyballWinD;
            else if (Dcounts < Scounts) broadcast = translation.ValleyballWinS;
            else broadcast = translation.ValleyballWinNo;

            broadcast += translation.ValleyballCounter;
            broadcast += translation.ValleyballEnd;

            Extensions.Broadcast(broadcast, 10);

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
