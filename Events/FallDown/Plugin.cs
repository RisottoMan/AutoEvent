using AutoEvent.Events.Speedrun.Features;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.FallDown
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Fall Down";
        public override string Description { get; set; } = "All platforms are destroyed. It is necessary to survive.[Beta]";
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "fall";
        public static SchematicObject GameMap { get; set; }
        public static TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;
        public GameObject Lava { get; set; }

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
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("FallDown", new Vector3(10f, 1020f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }

            Lava = GameMap.AttachedBlocks.First(x => x.name == "Lava");
            Lava.AddComponent<LavaComponent>();

            Timing.RunCoroutine(OnEventRunning(), "fall_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;

            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            List<GameObject> platformes = GameMap.AttachedBlocks.Where(x => x.name == "Platform").ToList();

            while (Player.List.Count(r => r.IsAlive) > 1 && platformes.Count > 1)
            {
                var count = Player.List.Count(r => r.IsAlive);
                var time = $"{EventTime.Minutes}:{EventTime.Seconds}";
                Extensions.Broadcast($"{Name}\n{time}\nОсталось {count} игроков", 1);

                var platform = platformes.RandomItem();
                platformes.Remove(platform);
                GameObject.Destroy(platform);

                yield return Timing.WaitForSeconds(0.9f);
                EventTime += TimeSpan.FromSeconds(0.9f);
            }

            if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast($"Победил игрок {Player.List.First(r => r.IsAlive).Nickname}", 10);
            }
            else
            {
                Extensions.Broadcast($"Все сдохли)))))\nКонец игры.", 10);
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
