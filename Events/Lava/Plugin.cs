using AutoEvent.Events.Lava.Features;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Toys;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Lava
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Пол - это ЛАВА [Testing]";
        public override string Description { get; set; } = "Выживание, в котором необходимо избегать лавы и стрелять в других. [Testing]";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "lava";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public Primitive Lava { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            OnEventStarted();

            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned += _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon += _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurt;
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurt;

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Server.FriendlyFire = true;

            GameMap = Extensions.LoadMap("Lava", new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            /*
            // Создание лавы
            Lava.Position = GameMap.Position;
            Lava.AddPart(new ModelPrimitive(LavaModel, PrimitiveType.Cube, new Color32(255, 0, 0, 255), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(100, 1, 100)));
            foreach (var prim in LavaModel.Primitives)
            {
                prim.GameObject.AddComponent<LavaComponent>();
            }
            */

            // Делаем всех д классами
            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.EnableEffect(EffectType.Ensnared);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "lava_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
            /*
            foreach(Player player in Player.List)
            {
                player.DisableEffect(EffectType.Ensnared);
                player.GameObject.AddComponent<BoxCollider>();
                player.GameObject.AddComponent<BoxCollider>().size = new Vector3(1f, 3f, 1f);
            }
            */
            while (Player.List.Count(r => r.IsAlive) > 1)
            {
                string text = string.Empty;
                if (EventTime.TotalSeconds % 2 == 0)
                {
                    text = "<size=90><color=red><b>《 ! 》</b></color></size>\n";
                }
                else
                {
                    text = "<size=90><color=red><b>!</b></color></size>\n";
                }
                Extensions.Broadcast(text + $"<size=20><color=red><b>Живых: {Player.List.Count(r => r.Role != RoleTypeId.Spectator)} Игроков</b></color></size>", 1);
                //Lava.Position += new Vector3(0, 0.1f, 0);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast($"<size=80><color=red><b>Победитель\n{Player.List.First(r => r.IsAlive).Nickname}</b></color></size>", 10);
            }
            else
            {
                Extensions.Broadcast($"<size=70><color=red><b>Все утонули в Лаве)))))</b></color></size>", 10);
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Server.FriendlyFire = false;
            //Lava.Destroy();
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
