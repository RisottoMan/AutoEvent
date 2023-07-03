using AutoEvent.Events.DeathParty.Features;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features.Objects;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using PlayerRoles;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.DeathParty
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Death Party [TESTING]";
        public override string Description { get; set; } = "Survive in grenade rain";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "death";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }

        EventHandler _eventHandler;
        int Stage { get; set; }

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

            Timing.CallDelayed(5f, EventEnd);
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("DeathParty", new Vector3(100f, 1012f, -40f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("Escape.ogg", 4, true, Name);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }
            Timing.RunCoroutine(OnEventRunning(), "death_run");
            Timing.RunCoroutine(OnGrenadeEvent(), "death_grenade");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.List.Count(r => r.IsAlive) > 0)
            {
                Extensions.Broadcast("<color=yellow>Уворачивайтесь от гранат!</color>\n" +
                    $"<color=green>Прошло {EventTime.Minutes}:{EventTime.Seconds} секунд</color>\n" +
                    $"<color=red>Осталось {Player.List.Count(r => r.IsAlive)} игроков</color>", 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast($"<color=red>Смертельная вечеринка</color>\n" +
                    $"<color=yellow>Выжило <color=red>{Player.List.Count(r => r.IsAlive)}</color> игроков.</color>\n" +
                    $"<color=#ffc0cb>{EventTime.Minutes}:{EventTime.Seconds}</color>", 10);
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                var player = Player.List.First(r => r.IsAlive);
                player.Health = 1000;
                Extensions.Broadcast($"<color=red>Смертельная вечеринка</color>\n" +
                    $"<color=yellow>ПОБЕДИТЕЛЬ - <color=red>{player.Nickname}</color></color>\n" +
                    $"<color=#ffc0cb>{EventTime.Minutes}:{EventTime.Seconds}</color>", 10);
            }
            else
            {
                Extensions.Broadcast($"<color=red>Смертельная вечеринка</color>\n" +
                    $"<color=yellow>Все погибли(((</color>\n" +
                    $"<color=#ffc0cb>{EventTime.Minutes}:{EventTime.Seconds}</color>", 10);
            }
        }
        public IEnumerator<float> OnGrenadeEvent()
        {
            Stage = 1;
            float fuse = 10f;
            float radius = 7f;
            float height = 20f;
            float count = 50;
            float timing = 1f;

            while (Player.List.Count(r => r.IsAlive) > 0) //
            {
                for (int i = 0; i < count; i++)
                {
                    GrenadeSpawn(fuse, radius, height);
                    yield return Timing.WaitForSeconds(timing);
                }

                yield return Timing.WaitForSeconds(15f);

                if (Stage < 4)
                {
                    fuse -= 2f;
                    height -= 5f;
                    timing -= 0.3f;
                }
                radius += 7f;
                count += 40;
                Stage++;
            }
            yield break;
        }

        public void GrenadeSpawn(float fuseTime, float radius, float height)
        {
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            //grenade.Scale  = new Vector3(2f, 3f, 2f);
            grenade.FuseTime = fuseTime;
            grenade.MaxRadius = radius;
            grenade.SpawnActive(GameMap.Position + new Vector3(Random.Range(-5, 5), height, Random.Range(-5, 5)));
            //grenade.Scale.Set(10f, 10f, 10f);
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
            Extensions.UnLoadMap(GameMap);
        }
    }
}
