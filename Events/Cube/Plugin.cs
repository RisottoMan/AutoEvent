using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Cube
{
    internal class Plugin// : Event
    {
        public string Name { get; set; } = AutoEvent.Singleton.Translation.CubeName;
        public string Description { get; set; } = AutoEvent.Singleton.Translation.CubeDescription;
        public string Color { get; set; } = "FF4242";
        public string CommandName { get; set; } = "cube";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        private List<GameObject> Cubes;

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
            EventTime = new TimeSpan(0, 2, 0);
            GameMap = Extensions.LoadMap("Cube", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("Cube.ogg", 10, true, Name);

            foreach(Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                player.Position = GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint").transform.position;
            }

            Timing.RunCoroutine(OnEventRunning(), "cube_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Cubes = new List<GameObject>();
            var fallCubes = GameMap.AttachedBlocks.Where(x => x.name == "CubePosition").ToList();
            var cube = GameMap.AttachedBlocks.First(x => x.name == "Cube_Default");

            while (Player.List.Count(r => r.Role == RoleTypeId.ClassD) > 0 && EventTime.TotalSeconds > 0)
            {
                var _cube = GameObject.Instantiate(cube, fallCubes.RandomItem().transform.position + new Vector3(0f, -5f, 0f), Quaternion.Euler(Vector3.zero));
                NetworkServer.Spawn(_cube);
                Cubes.Add(_cube);

                yield return Timing.WaitForSeconds(1f);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
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
