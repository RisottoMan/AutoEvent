using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoEvent.Events.FallCubes
{
    internal class Plugin //: Event
    {
        public override string CommandName { get; set; } = "cube";
        public override string Name { get; set; } = "Fall Cube";
        public override string Description { get; set; } = "Fall cubes";
        public override string Color { get; set; } = "b532d9";
        public EventHandler _eventHandler;

        public static SchematicObject GameMap;
        private GameObject DefaultCube;
        private List<GameObject> Cubes = new List<GameObject>();
        private Vector3 SpawnPosition;
        private TimeSpan EventTime;
        private TimeSpan RoundTime;
        private int RoundCounter = 1;

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

        private void OnEventStarted()
        {
            GameMap = Extensions.LoadMap("FallCubes", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            SpawnPosition = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            DefaultCube = GameMap.AttachedBlocks.First(x => x.name == "DefaultCube");
            EventTime = TimeSpan.Zero;

            Player.List.ToList().ForEach(pl =>
            {
                pl.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.AssignInventory);
                pl.Position = SpawnPosition;
            });

            ResetRoundTime();
            Timing.RunCoroutine(OnEventRunning(), "fallcubes_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            var FallCubePositions = GameMap.AttachedBlocks.Where(x => x.name == "CubePosition").ToList();

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(trans.TimerBeforeStart.Replace("%time%", $"{time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0)
            {
                Extensions.Broadcast($"{EventTime.Seconds}", 1);

                if (Cubes.Count <= 3)
                {
                    var _cube = GameObject.Instantiate(DefaultCube, FallCubePositions.RandomItem().transform.position - new Vector3(0, 5f, 0), Quaternion.Euler(Vector3.zero));

                    _cube.AddComponent<CubeComponent>();
                    NetworkServer.Spawn(_cube);
                    Cubes.Add(_cube);
                }

                EventTime += TimeSpan.FromSeconds(1f);
                RoundTime -= TimeSpan.FromSeconds(1f);
                yield return Timing.WaitForSeconds(1f);
            }

            OnStop();
            yield break;
        }

        private void ResetRoundTime() { RoundTime = TimeSpan.FromSeconds(30f); }

        private void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Cubes.ForEach(GameObject.Destroy);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
