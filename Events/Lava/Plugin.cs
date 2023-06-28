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

            _eventHandler = new EventHandler(this);

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

            GameMap = Extensions.LoadMap("Lava.json", new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("FallGuys_DnB.ogg", 5, true, Name); // ???

            /*
            // Переписать
            for (int i = 0; i < 20; i++)
            {
                var item = ItemType.GunCOM15;
                var rand = Random.Range(0, 100);
                if (rand < 40) item = ItemType.GunCOM15;
                else if (rand >= 40 && rand < 80) item = ItemType.GunCOM18;
                else if (rand >= 80 && rand < 90) item = ItemType.GunRevolver;
                else if (rand >= 90 && rand < 100) item = ItemType.GunFSP9;
                Pickup pickup = new Item(item).Spawn(GameMap.GameObject.transform.position + new Vector3(Random.Range(-30, 31), 30, Random.Range(-30, 31)));
            }
            */

            // Делаем всех д классами
            foreach (var player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD);
                player.EnableEffect(EffectType.Ensnared);
                player.Position = GameMap.Position + RandomClass.GetRandomPosition();
            }

            Timing.RunCoroutine(OnEventRunning(), "battle_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
            Player.List.ToList().ForEach(player =>
            {
                player.DisableEffect(EffectType.Ensnared);
                player.GameObject.AddComponent<BoxCollider>();
                player.GameObject.AddComponent<BoxCollider>().size = new Vector3(1f, 3f, 1f);
            });

            /*
            // Создание лавы
            Lava.Position = GameMap.Position;
            Lava.AddPart(new ModelPrimitive(LavaModel, PrimitiveType.Cube, new Color32(255, 0, 0, 255), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(100, 1, 100)));
            foreach (var prim in LavaModel.Primitives)
            {
                prim.GameObject.AddComponent<LavaComponent>();
            }
            */

            while (Player.List.Count(r => r.Role != RoleTypeId.Spectator) > 1)
            {
                string text = string.Empty;
                if (EventTime.TotalSeconds % 2 == 0)
                {
                    text = "<size=90><color=red><b>《 ! 》</b></color></size>\n";
                }
                else
                {
                    text = "<size=90><color=red><b>  !  </b></color></size>\n";
                }
                Extensions.Broadcast(text + $"<size=20><color=red><b>Живых: {Player.List.ToList().Count(r => r.Role != RoleTypeId.Spectator)} Игроков</b></color></size>", 1);
                Lava.Position += new Vector3(0, 0.1f, 0);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
            if (Player.List.ToList().Count(r => r.Role != RoleTypeId.Spectator) == 1)
            {
                Extensions.Broadcast($"<size=80><color=red><b>Победитель\n{Player.List.ToList().First(r => r.Role != RoleTypeId.Spectator).Nickname}</b></color></size>", 10);
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
            Lava.Destroy();
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
