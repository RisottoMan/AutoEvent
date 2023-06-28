using AutoEvent.Events.Infection;
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
        public override string Name { get; set; } = "Мясная Заруба [Test]";
        public override string Description { get; set; } = "Битва, в которой одна из команд должна одолеть другую. [Test]";
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
            GameMap = Extensions.LoadMap("Battle.json", new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            //Extensions.PlayAudio("MGS4.ogg", 3, true, Name); // ???? найти подходящую музыку

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                    RandomClass.CreateSoldier(player);
                    player.Position = RandomPosition.GetSpawnPosition(GameMap); // ???? как понять кого куда спавнить
                }
                else
                {
                    player.Role.Set(RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
                    RandomClass.CreateSoldier(player);
                    player.Position = RandomPosition.GetSpawnPosition(GameMap); // ????
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
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            Player.List.ToList().ForEach(player => player.DisableEffect(EffectType.Ensnared));
            while (Player.List.Count(r => r.Role.Team == Team.FoundationForces) > 0 && Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) > 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Заруба</i></b></color>\n" +
                $"<color=yellow><color=blue>{Player.List.Count(r => r.Role.Team == Team.FoundationForces)}</color> " +
                $"VS <color=green>{Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency)}</color></color>\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.Role.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Заруба</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИЛИ - <color=green>{Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency)} ХАОС</color></color>\n" +
                $"<color=yellow>Конец ивент: <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 10);
            }
            else if (Player.List.Count(r => r.Role.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Заруба</i></b></color>\n" +
                $"<color=yellow>ПОБЕДИЛИ - <color=blue>{Player.List.Count(r => r.Role.Team == Team.FoundationForces)} МОГ</color></color>\n" +
                $"<color=yellow>Конец ивент: <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 10);
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
