using AutoEvent.Events.Jump.Features;
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

namespace AutoEvent.Events.Jump
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Last Jump";
        public override string Description { get; set; } = "Нужно прыгать через крутяющую балку и выжить.";
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "jump";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

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

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            GameMap = Extensions.LoadMap("Jump", new Vector3(20f, 1026.5f, -45f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("LineLite.ogg", 10, true, Name);

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.Scientist, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;
            }

            Timing.RunCoroutine(OnEventRunning(), "jump_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            EventTime = new TimeSpan(0, 2, 0);

            GameMap.AttachedBlocks.First(r => r.name == "Lava").AddComponent<LavaComponent>();

            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"Быстрее дойдите до конца!!!\nУ вас осталось {time} секунд.", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            GameObject main = new GameObject();
            foreach(var platform in GameMap.AttachedBlocks)
            {
                switch(platform.name)
                {
                    case "Platform": GameObject.Destroy(platform); break;
                    case "Redline": platform.AddComponent<LavaComponent>(); break;
                    case "RotateLine": platform.AddComponent<RotateComponent>(); break;
                    case "RotateRedline": platform.AddComponent<RotateRedComponent>(); break;
                    case "Main": main = platform; break;
                }
            }

            while (Player.List.Count(r => r.IsAlive) > 1 && EventTime.TotalSeconds > 0)
            {
                Extensions.Broadcast($"{Name}\nДо конца - {EventTime.Minutes}:{EventTime.Seconds}\nОсталось игроков - {Player.List.Count(r=>r.IsAlive)}", 1);

                if (EventTime.Minutes == 1 && EventTime.Seconds == 0)
                {
                    main.AddComponent<PlatformLowering>();
                }

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }    

            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast($"Выжило {Player.List.Count(r=>r.IsAlive)} игроков.\nПобеда", 10);
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast($"Победил игрок {Player.List.First(r => r.IsAlive).Nickname}\nПоздравляем тебя!", 10);
            }
            else
            {
                Extensions.Broadcast($"Никто не выжил.\nКонец мини-игры", 10);
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