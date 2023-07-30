using AutoEvent.Events.Boss.Features;
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
using Utils.NonAllocLINQ;

namespace AutoEvent.Events.Boss
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.BossName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.BossDescription;
        public override string MapName { get; set; } = "DeathParty";
        public override string CommandName { get; set; } = "boss";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public List<GameObject> Workstations { get; set; }

        EventHandler _eventHandler;

        Player Boss;

        public override void OnStart()
        {
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

            OnEventStarted();
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

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 2, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(6f, 1030f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.NtfSergeant, SpawnReason.None, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
                player.Health = 200;

                RandomClass.CreateSoldier(player);
                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = player.Items.First();
                });
            }

            Timing.RunCoroutine(OnEventRunning(), "battle_time");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.BossTimeLeft.Replace("{time}", $"{time}")}", 5);
                yield return Timing.WaitForSeconds(1f);
            }

            Extensions.PlayAudio("Boss.ogg", 7, false, Name);

            Boss = Player.List.Where(r => r.IsNTF).ToList().RandomItem();
            Boss.Role.Set(RoleTypeId.ChaosConscript, SpawnReason.None, RoleSpawnFlags.None);
            Boss.Position = RandomClass.GetSpawnPosition(GameMap);

            Boss.Health = Player.List.Count() * 4000;
            Boss.Scale = new Vector3(5, 5, 5);

            Boss.ClearInventory();
            Boss.AddItem(ItemType.GunLogicer);
            Timing.CallDelayed(0.1f, () => { Boss.CurrentItem = Boss.Items.First(); });

            while (EventTime.TotalSeconds > 0 && Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0 && Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) > 0)
            {
                var text = AutoEvent.Singleton.Translation.BossCounter;
                text = text.Replace("%hp%", $"{(int)Boss.Health}");
                text = text.Replace("%count%", $"{Player.List.Count(r => r.IsNTF)}");
                text = text.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}");

                Extensions.Broadcast(text, 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
            {
                Boss.Scale = new Vector3(1, 1, 1);
                Extensions.Broadcast(AutoEvent.Singleton.Translation.BossWin.Replace("%hp%", $"{(int)Boss.Health}"), 10);
            }
            else if (Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) == 0)
            {
                Boss.Hurt(15000, "Boss");
                Extensions.Broadcast(AutoEvent.Singleton.Translation.BossHumansWin.Replace("%count%", $"{Player.List.Count(r => r.IsNTF)}"), 10);
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
