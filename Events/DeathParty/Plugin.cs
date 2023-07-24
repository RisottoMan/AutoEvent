using AutoEvent.Events.DeathParty.Features;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features.Objects;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerRoles;
using Random = UnityEngine.Random;
using AutoEvent.Interfaces;

namespace AutoEvent.Events.DeathParty
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.DeathName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.DeathDescription;
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "death";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }

        EventHandler _eventHandler;
        private bool isFreindlyFireEnabled;
        int Stage { get; set; }
        int MaxStage { get; set; }

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = true;

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
            Server.FriendlyFire = isFreindlyFireEnabled;

            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            Timing.CallDelayed(5f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            MaxStage = 5;
            GameMap = Extensions.LoadMap("DeathParty", new Vector3(100f, 1012f, -40f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Escape.ogg", 4, true, Name);

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
            for (int _time = 10; _time > 0; _time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{_time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.List.Count(r => r.IsAlive) > 0 && Stage != (MaxStage + 1))
            {
                var count = Player.List.Count(r => r.IsAlive).ToString();
                var cycleTime = $"{EventTime.Minutes}:{EventTime.Seconds}";
                Extensions.Broadcast(AutoEvent.Singleton.Translation.DeathCycle.Replace("%count%", count).Replace("%time%", cycleTime), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            var time = $"{EventTime.Minutes}:{EventTime.Seconds}";
            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.DeathMorePlayer.Replace("%count%", $"{Player.List.Count(r => r.IsAlive)}").Replace("%time%", time), 10);
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                var player = Player.List.First(r => r.IsAlive);
                player.Health = 1000;
                Extensions.Broadcast(AutoEvent.Singleton.Translation.DeathOnePlayer.Replace("%winner%", player.Nickname).Replace("%time%", time), 10);
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.DeathAllDie.Replace("%time%", time), 10);
            }

            OnStop();
            yield break;
        }
        public IEnumerator<float> OnGrenadeEvent()
        {
            Stage = 1;
            float fuse = 10f;
            float height = 20f;
            float count = 20;
            float timing = 1f;
            float scale = 4;
            float radius = GameMap.AttachedBlocks.First(x => x.name == "Arena").transform.localScale.x / 2;

            while (Player.List.Count(r => r.IsAlive) > 0 && Stage != (MaxStage + 1))
            {
                if (Stage < MaxStage)
                {
                    for (int i = 0; i < count; i++)
                    {
                        GrenadeSpawn(fuse, radius, height, scale);
                        yield return Timing.WaitForSeconds(timing);
                    }
                }
                else
                {
                    GrenadeSpawn(fuse, radius, height, 100);
                }

                yield return Timing.WaitForSeconds(15f);

                fuse -= 2f;
                height -= 5f;
                timing -= 0.3f;
                radius += 7f;
                count += 20;
                scale -= 1;
                Stage++;
            }
            yield break;
        }

        public void GrenadeSpawn(float fuseTime, float radius, float height, float scale) // Пожирнее и помедленее // нанесение урона
        {
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = fuseTime;
            grenade.MaxRadius = radius;

            var projectile = grenade.SpawnActive(GameMap.Position + new Vector3(Random.Range(-radius, radius), height, Random.Range(-radius, radius)));
            projectile.Weight = 1000f;
            projectile.Scale = new Vector3(scale, scale, scale);
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
            Extensions.UnLoadMap(GameMap);
            AutoEvent.ActiveEvent = null;
        }
    }
}
