using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;
using Component = AutoEvent.Events.Puzzle.Features.Component;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.Puzzle
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Puzzle";
        public override string Description { get; set; } = "alpha testing";
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
            Lava.AddComponent<Component>();

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = GameMap.Position + new Vector3(0, 8.2f, 0);
            }

            Timing.RunCoroutine(OnEventRunning(), "puzzle_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            int stage = 1;
            int speed = 5;
            float timing = 0.5f;

            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"Ивент {Name}\nДо начала игры <color=red>{time}</color> секунд.", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            List<GameObject> ListPlatformes = Platformes;
            while (stage <= 5 && Player.List.Count(r=>r.IsAlive) > 0)
            {
                for (int time = speed * 2; time > 0; time--)
                {
                    foreach (var platform in Platformes)
                    {
                        platform.GetComponent<PrimitiveObject>().Primitive.Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                    }
                    Extensions.Broadcast($"Ивент {Name}\nЭтап {stage}\nОсталось {Player.List.Count(r => r.IsAlive)} игроков", 1);
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
                Extensions.Broadcast($"Ивент {Name}\nЭтап {stage}\nОсталось {Player.List.Count(r => r.IsAlive)} игроков", (ushort)speed);
                yield return Timing.WaitForSeconds(speed);

                foreach (var platform in Platformes)
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.down * 5;
                    }
                }
                Extensions.Broadcast($"Ивент {Name}\nЭтап {stage}\nОсталось {Player.List.Count(r => r.IsAlive)} игроков", (ushort)speed);
                yield return Timing.WaitForSeconds(speed);

                foreach (var platform in Platformes)
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.up * 5;
                    }
                }
                Extensions.Broadcast($"Ивент {Name}\nЭтап {stage}\nОсталось {Player.List.Count(r => r.IsAlive)} игроков", (ushort)speed);
                yield return Timing.WaitForSeconds(speed);

                speed--;
                stage++;
                timing -= 0.1f;
            }

            if (Player.List.Count(r => r.IsAlive) < 1)
            {
                Extensions.Broadcast($"<color=red>Никто не выжил.\nКонец мини-игры.</color>", 10);
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
