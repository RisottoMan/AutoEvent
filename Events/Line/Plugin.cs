using AutoEvent.Events.Line.Features;
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

namespace AutoEvent.Events.Line
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.LineName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.LineDescription;
        public override string Author { get; set; } = "Logic_Gun";
        public override string MapName { get; set; } = "Line";
        public override string CommandName { get; set; } = "line";
        public static SchematicObject GameMap { get; set; }
        public Dictionary<int, SchematicObject> HardGameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        private int HardCounts;
        private int HardCountsLimit = 8;

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

            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            HardGameMap = new Dictionary<int, SchematicObject>();
            HardCounts = 0;
            EventTime = TimeSpan.FromMinutes(5f);

            GameMap = Extensions.LoadMap(MapName, new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);

            Extensions.PlayAudio("LineLite.ogg", 10, true, Name);

            Player.List.ToList().ForEach(pl =>
            {
                pl.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                pl.Position = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            });
            Timing.RunCoroutine(OnEventRunning(), "line_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            foreach (var block in GameMap.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "DeadZone": block.AddComponent<LineComponent>(); break;
                    case "DeadWall": block.AddComponent<LineComponent>(); break;
                    case "Line": block.AddComponent<LineComponent>(); break;
                    case "Shield": GameObject.Destroy(block); break;
                }
            }

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 1 && EventTime.TotalSeconds > 0)
            {
                Extensions.Broadcast(trans.LineBroadcast.Replace("%name%", Name)
                    .Replace("%time%", $"{EventTime.Minutes} {EventTime.Seconds}")
                    .Replace("%alive%", $"{Player.List.Count(r => r.Role == RoleTypeId.ClassD)}"), 1);

                if (EventTime.Seconds == 30 && HardCounts < HardCountsLimit)
                {
                    if (HardCounts == 0)
                    {
                        Extensions.StopAudio();
                        Extensions.PlayAudio("LineHard.ogg", 10, true, Name);
                    }

                    try
                    {
                        var map_hard = Extensions.LoadMap("HardLine", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
                        HardGameMap.Add(HardCounts, map_hard);
                    }
                    catch(Exception ex)
                    {
                        Log.Info($"{ex}");
                    }
                    HardCounts++;
                }

                EventTime -= TimeSpan.FromSeconds(1f);
                yield return Timing.WaitForSeconds(1f);
            }    

            if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 1)
            {
                Extensions.Broadcast(trans.LineWinners.Replace("%name%", Name)
                    .Replace("%alive%", $"{Player.List.Count(x => x.Role == RoleTypeId.ClassD)}"), 10);
            }
            else if (Player.List.Count(r => r.Role == RoleTypeId.ClassD) == 1)
            {
                Extensions.Broadcast(trans.LineWinner.Replace("%name%", Name)
                    .Replace("%nickname", $"{Player.List.First(r => r.Role == RoleTypeId.ClassD)}"), 10);
            }
            else
            {
                Extensions.Broadcast(trans.LineAllDied, 10);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            foreach (var map in HardGameMap.Values) Extensions.UnLoadMap(map);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}