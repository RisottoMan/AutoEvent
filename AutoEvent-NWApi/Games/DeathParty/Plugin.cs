using AutoEvent.Games.DeathParty.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using InventorySystem.Items.ThrowableProjectiles;
using AutoEvent.API.Schematic.Objects;
using InventorySystem.Items;
using AutoEvent.Events.Handlers;
using Event = AutoEvent.Interfaces.Event;
using Mirror;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using InventorySystem.Items.Pickups;

namespace AutoEvent.Games.DeathParty
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.DeathName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.DeathDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "DeathParty";
        public override string CommandName { get; set; } = "death";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }

        EventHandler _eventHandler;

        private bool isFreindlyFireEnabled;
        public static int Stage { get; set; }
        public int MaxStage { get; set; }

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = true;

            _eventHandler = new EventHandler();

            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;

            OnEventStarted();
        }

        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            Timing.CallDelayed(5f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            MaxStage = 5;
            GameMap = Extensions.LoadMap(MapName, new Vector3(10f, 1012f, -40f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("DeathParty.ogg", 5, true, Name);

            foreach (Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "death_run");
            Timing.RunCoroutine(OnGrenadeEvent(), "death_grenade");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;

            for (int _time = 10; _time > 0; _time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{_time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (Player.GetPlayers().Count(r => r.IsAlive) > 0 && Stage <= MaxStage)
            {
                var count = Player.GetPlayers().Count(r => r.IsAlive).ToString();
                var cycleTime = $"{EventTime.Minutes}:{EventTime.Seconds}";
                Extensions.Broadcast(translation.DeathCycle.Replace("%count%", count).Replace("%time%", cycleTime), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            var time = $"{EventTime.Minutes}:{EventTime.Seconds}";
            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(translation.DeathMorePlayer.Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsAlive)}").Replace("%time%", time), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                var player = Player.GetPlayers().First(r => r.IsAlive);
                player.Health = 1000;
                Extensions.Broadcast(translation.DeathOnePlayer.Replace("%winner%", player.Nickname).Replace("%time%", time), 10);
            }
            else
            {
                Extensions.Broadcast(translation.DeathAllDie.Replace("%time%", time), 10);
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
            float radius = GameMap.AttachedBlocks.First(x => x.name == "Arena").transform.localScale.x / 2 - 6f;

            while (Player.GetPlayers().Count(r => r.IsAlive) > 0 && Stage <= MaxStage)
            {
                if (Stage != MaxStage)
                {
                    for (int i = 0; i < count; i++)
                    {
                        GrenadeSpawn(fuse, radius, height, scale);
                        yield return Timing.WaitForSeconds(timing);
                    }
                }
                else
                {
                    GrenadeSpawn(10, 10, 20f, 75);
                }

                yield return Timing.WaitForSeconds(15f);

                fuse -= 2f;
                height -= 5f;
                timing -= 0.3f;
                radius += 7f;
                count += 30;
                scale -= 1;
                Stage++;
            }
            yield break;
        }
        
        public void GrenadeSpawn(float fuseTime, float radius, float height, float scale)
        {
            var item = CreateThrowable(ItemType.GrenadeHE);

            Vector3 pos = GameMap.Position + new Vector3(Random.Range(-radius, radius), height, Random.Range(-radius, radius));

            TimeGrenade grenadeboom = (TimeGrenade)Object.Instantiate(item.Projectile, pos, Quaternion.identity);
            grenadeboom._fuseTime = fuseTime;
            grenadeboom.NetworkInfo = new PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
            grenadeboom.transform.localScale = new Vector3(scale, scale, scale);

            NetworkServer.Spawn(grenadeboom.gameObject);
            grenadeboom.ServerActivate();
        }

        public static ThrowableItem CreateThrowable(ItemType type, Player player = null) => (player != null ? player.ReferenceHub : ReferenceHub.HostHub)
        .inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false) as ThrowableItem;

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
