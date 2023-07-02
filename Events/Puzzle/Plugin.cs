using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;
using Exiled.API.Enums;
using AutoEvent.Events.Puzzle.Features;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.Puzzle
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Puzzle";
        public override string Description { get; set; } = "Get up the fastest on the right color.";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "puzzle";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        public List<GameObject> Platformes { get; set; }
        public GameObject Lava { get; set; }

        EventHandler _eventHandler;

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

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            GameMap = Extensions.LoadMap("Puzzle", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            Platformes = GameMap.AttachedBlocks.Where(x => x.name == "Platform").ToList();
            Lava = GameMap.AttachedBlocks.First(x => x.name == "Lava");
            Lava.AddComponent<LavaComponent>();

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, SpawnReason.None ,RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "puzzle_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"{Name}\nДо начала игры <color=red>{time}</color> секунд.", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            int stage = 1;
            int finaleStage = 10;
            float speed = 5;
            float timing = 0.5f;
            List<GameObject> ListPlatformes = Platformes;

            while (stage <= finaleStage && Player.List.Count(r=>r.IsAlive) > 0)
            {
                for (float time = speed * 2; time > 0; time--)
                {
                    foreach (var platform in Platformes)
                    {
                        platform.GetComponent<PrimitiveObject>().Primitive.Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                    }
                    Extensions.Broadcast($"<b>{Name}</b>\nЭтап <color=red>{stage}/{finaleStage}</color>\nОсталось <color=green>{Player.List.Count(r => r.IsAlive)} игроков</color>", 1);
                    yield return Timing.WaitForSeconds(timing);
                }

                var randPlatform = ListPlatformes.RandomItem();
                ListPlatformes = new List<GameObject>();
                randPlatform.GetComponent<PrimitiveObject>().Primitive.Color = UnityEngine.Color.green;

                foreach (var platform in Platformes)
                {
                    if (platform != randPlatform)
                    {
                        platform.GetComponent<PrimitiveObject>().Primitive.Color = UnityEngine.Color.magenta;
                        ListPlatformes.Add(platform);
                    }
                }
                Extensions.Broadcast($"<b>{Name}</b>\nЭтап <color=red>{stage}/{finaleStage}</color>\n" +
                    $"Осталось <color=green>{Player.List.Count(r => r.IsAlive)} игроков</color>", (ushort)(speed + 1));
                yield return Timing.WaitForSeconds(speed);

                foreach (var platform in Platformes)
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.down * 5;
                    }
                }
                Extensions.Broadcast($"<b>{Name}</b>\nЭтап <color=red>{stage}/{finaleStage}</color>\n" +
                    $"Осталось <color=green>{Player.List.Count(r => r.IsAlive)} игроков</color>", (ushort)(speed + 1));
                yield return Timing.WaitForSeconds(speed);

                foreach (var platform in Platformes)
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.up * 5;
                    }
                }
                Extensions.Broadcast($"<b>{Name}</b>\nЭтап <color=red>{stage}/{finaleStage}</color>\n" +
                    $"Осталось <color=green>{Player.List.Count(r => r.IsAlive)} игроков</color>", (ushort)(speed + 1));
                yield return Timing.WaitForSeconds(speed);

                speed -= 0.35f;
                stage++;
                timing -= 0.035f;
            }

            if (Player.List.Count(r => r.IsAlive) < 1)
            {
                Extensions.Broadcast($"<color=red>Никто не выжил.\nКонец мини-игры.</color>", 10);
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast($"<color=green>Победил игрок <b>{Player.List.Count(r => r.IsAlive)}</b>.\nКонец мини-игры.</color>", 10);
            }
            else
            {
                Extensions.Broadcast($"<color=green>Поздравляем всех выживших с победой.\nВыжило {Player.List.Count(r=>r.IsAlive)} игроков.</color>", 10);
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
