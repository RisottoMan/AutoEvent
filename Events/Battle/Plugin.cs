using AutoEvent.Commands;
using AutoEvent.Events.Battle.Features;
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

namespace AutoEvent.Events.Battle
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Battle [Testing]";
        public override string Description { get; set; } = "MTF vs CI [Testing]";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "battle";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }

        EventHandler _eventHandler;

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

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("Battle", new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                    RandomClass.CreateSoldier(player);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
                    RandomClass.CreateSoldier(player);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;
            }
            Timing.RunCoroutine(OnEventRunning(), "battle_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            Player.List.ToList().ForEach(player => player.EnableEffect(EffectType.Ensnared));
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{AutoEvent.Singleton.Translation.BattleTimeLeft.Replace("{time}", $"{time}")}", 5);
                yield return Timing.WaitForSeconds(1f);
            }

            Player.List.ToList().ForEach(player => player.DisableEffect(EffectType.Ensnared));

            while (Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0 && Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) > 0)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.BattleCounter.Replace("{FoundationForces}", $"{Player.List.Count(r => r.Role.Team == Team.FoundationForces)}").Replace("{ChaosForces}", $"{Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency)}").Replace("{EvTime}", $"{EventTime.Minutes}:{EventTime.Seconds}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(
                    $"{AutoEvent.Singleton.Translation.BattleCiWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}")}",
                    3);
            }
            else if (Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.BattleMtfWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
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
