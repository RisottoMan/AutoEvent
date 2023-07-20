using AutoEvent.Events.Line.Features;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.Events.Commands.Reload;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.Configs;
using MEC;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Cube
{
    internal class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.CubeName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.CubeDescription;
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "cube";
        public static SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        private List<Vector3> CubePosition;
        private GameObject Cube_Default;
        private List<GameObject> Cubes;

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
            EventTime = TimeSpan.FromMinutes(2f);
            GameMap = Extensions.LoadMap("Cube", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            CubePosition = new List<Vector3>();

            Cube_Default = GameMap.AttachedBlocks.First(x => x.name == "Cube_Default");

            GameMap.AttachedBlocks.Where(x => x.name == "CubePosition").ToList().ForEach(cube => CubePosition.Add(cube.transform.position));

            //Extensions.PlayAudio("Cube.ogg", 10, true, Name);

            Player.List.ToList().ForEach(pl =>
            {
                pl.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.AssignInventory);
                pl.Position = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            });

            Timing.RunCoroutine(OnEventRunning(), "cube_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) >= 1 && EventTime.TotalSeconds > 0)
            {
                var cube = GameObject.Instantiate(Cube_Default, CubePosition.RandomItem() + new Vector3(0f, -5f, 0f), Quaternion.Euler(Vector3.zero));
                Cubes.Add(cube);
                NetworkServer.Spawn(cube);
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Cubes.ForEach(UnityEngine.Object.Destroy);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
