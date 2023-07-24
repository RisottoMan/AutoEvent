using AutoEvent.Events.FinishWay.Features;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.FinishWay
{
    public class Plugin// : Event
    {
        public string Name { get; set; } = AutoEvent.Singleton.Translation.FinishWayName;
        public string Description { get; set; } = AutoEvent.Singleton.Translation.FinishWayDescription;
        public string Color { get; set; } = "FF4242";
        public string CommandName { get; set; } = "finish";
        public static SchematicObject GameMap { get; set; }
        public static TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;
        public GameObject Lava { get; set; }

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

        public void OnEventStarted()
        {
            GameMap = Extensions.LoadMap("FinishWay", new Vector3(115.5f, 1030f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }

            Lava = GameMap.AttachedBlocks.First(x => x.name == "Lava");
            Lava.AddComponent<LavaComponent>();

            Timing.RunCoroutine(OnEventRunning(), "finish_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"Ивент {Name}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            EventTime = new TimeSpan(0, 2, 0);
            while (Player.List.Count(r => r.IsAlive) > 0 && EventTime.TotalSeconds != 0)
            {
                var count = Player.List.Count(r => r.Role == RoleTypeId.ClassD);
                var time = $"{EventTime.Minutes}:{EventTime.Seconds}";

                Extensions.Broadcast(AutoEvent.Singleton.Translation.FinishWayCycle.Replace("%name%", Name).Replace("time", time), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            var point = GameMap.AttachedBlocks.First(x => x.name == "FinishTrigger");

            foreach(Player player in Player.List)
            {
                if (Vector3.Distance(player.Position, point.transform.position) > 10)
                {
                    player.Kill(AutoEvent.Singleton.Translation.FinishWayDied);
                }
            }

            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.FinishWaySeveralSurvivors.Replace("%count%", Player.List.Count(r => r.IsAlive).ToString()), 10);
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.FinishWayOneSurvived.Replace("%player%", Player.List.First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.FinishWayNoSurvivors, 10);
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
