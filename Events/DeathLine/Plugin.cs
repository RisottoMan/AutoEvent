using AutoEvent.Events.DeathLine.Features;
using AutoEvent.Events.Speedrun.Features;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Commands.Reload;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.Configs;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.DeathLine
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.LineName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.LineDescription;
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "deathline";
        public static SchematicObject GameMap { get; set; }
        public static SchematicObject HardGameMap { get; set; }
        public static SchematicObject ShieldMap { get; set; }
        public GameObject DeadZone { get; set; }
        public GameObject DeadWall { get; set; }
        public GameObject Line { get; set; }
        public static TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        private int HardCounts = 0;
        private int HardCountsLimit = 8; // Setting

        public override void OnStart()
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

        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            EventEnd();
        }

        public void OnEventStarted()
        {
            GameMap = Extensions.LoadMap("DeathLine", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            ShieldMap = Extensions.LoadMap("ShieldLine", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);

            DeadZone = GameMap.AttachedBlocks.First(x => x.name == "DeadZone");
            DeadWall = GameMap.AttachedBlocks.First(x => x.name == "DeadWall");
            Line = GameMap.AttachedBlocks.First(x => x.name == "Line");
            DeadZone.AddComponent<LineComponent>();
            DeadWall.AddComponent<LineComponent>();
            Line.AddComponent<LineComponent>();

            Extensions.PlayAudio("LineLite.ogg", 10, true, Name);

            Player.List.ToList().ForEach(p =>
            {
                p.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                p.Position = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            });
            Timing.RunCoroutine(OnEventRunning(), "deathline_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            ShieldMap.Destroy();

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) >= 1) // Изменить на > 1
            {
                Extensions.Broadcast(trans.LineBroadcast.Replace("%name%", Name).Replace("%time%", $"<color=blue>{EventTime.Minutes}:{EventTime.Seconds}</color>").Replace("%count%", $"{Player.List.ToList().Count(r => r.Role == RoleTypeId.ClassD)}"), 1);

                if (EventTime.Seconds == 30)
                {
                    if (HardCounts < HardCountsLimit) HardGameMap = Extensions.LoadMap("HardLine", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);

                    if (HardCounts == 0 || HardCounts % 3 == 0)
                    {
                        Extensions.StopAudio();
                        Extensions.PlayAudio("LineHard.ogg", 10, true, Name);
                    }
                    HardCounts++;
                }

                EventTime += TimeSpan.FromSeconds(1f);
                yield return Timing.WaitForSeconds(1f);
            }    

            if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 1)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.LineWinner.Replace("%winner%", Player.List.First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.LineAllDied, 10);
            }

            _eventHandler = null;

            EventEnd();
            yield break;
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.UnLoadMap(HardGameMap);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}