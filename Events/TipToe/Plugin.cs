using AutoEvent.Events.TipToe.Features;
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

namespace AutoEvent.Events.TipToe
{
    public class Plugin//: Event
    {
        public string Name { get; set; } = "Tip Toe";
        public string Description { get; set; } = "Не наступайте на ложные плитки и найдите скрытый путь к финишу!";
        public string Color { get; set; } = "FF4242";
        public string CommandName { get; set; } = "tip";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

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
            GameMap = Extensions.LoadMap("TipToe", new Vector3(20f, 1026.5f, -45f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("LineLite.ogg", 10, true, Name);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "jump_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            EventTime = new TimeSpan(0, 1, 0);

            List<GameObject> list = GameMap.AttachedBlocks.Where(x => x.name == "Platform").ToList();

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(time.ToString(), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            GameObject.Destroy(GameMap.AttachedBlocks.First(x => x.name == "Wall"));

            while (Player.List.Count(r => r.IsAlive) > 1 && EventTime.TotalSeconds > 0)
            {
                Extensions.Broadcast($"<color=#{Color}>{Name}</color>\n" +
                    $"<color=blue>Осталось {EventTime.Minutes}:{EventTime.Seconds}</color>\n" +
                    $"<color=yellow>Осталось игроков - {Player.List.Count(r=>r.IsAlive)}</color>", 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            var finish = GameMap.AttachedBlocks.First(x => x.name == "Finish");
            foreach (Player player in Player.List)
            {
                if (Vector3.Distance(player.Position, finish.transform.position) >= 10)
                {
                    player.Hurt(500, AutoEvent.Singleton.Translation.GlassDied);
                }
            }

            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast($"<color=yellow>Выжило {Player.List.Count(r=>r.IsAlive)} игроков</color>\n<color=red>Победа</color>", 10);
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast($"</color=green>Победил игрок {Player.List.First(r => r.IsAlive).Nickname}</color>\n<color=yellow>Поздравляем тебя!</color>", 10);
            }
            else
            {
                Extensions.Broadcast($"<color=red>Никто не выжил.</color>\n<color=yellow>Конец мини-игры</color>", 10);
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