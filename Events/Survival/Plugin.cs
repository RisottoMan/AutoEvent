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

namespace AutoEvent.Events.Survival
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Zombie Survival [Testing]";
        public override string Description { get; set; } = "Survival of humans against zombies. [Alpha]";
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "zombie2";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        private bool isFreindlyFireEnabled;
        EventHandler _eventHandler;

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

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
        }
        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

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
            EventTime = new TimeSpan(0, 5, 0);

            GameMap = Extensions.LoadMap("Survival", new Vector3(115.5f, 1035f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            //Extensions.PlayAudio("Survival.ogg", 5, false, "Выживание");

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.NtfSergeant, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            //Timing.RunCoroutine(TimingBeginEvent($"Выживание", 15), "survival_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (float _time = 15; _time > 0; _time--)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>{Name}</i></b></color>\n<color=#ABF000>До начала заражения осталось <color=red>{_time}</color> секунд.</color>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            for (int i = 0; i <= Player.List.Count() / 10; i++)
            {
                Player.List.ToList().RandomItem().Role.Set(RoleTypeId.Scp0492, SpawnReason.Revived, RoleSpawnFlags.AssignInventory);
            }

            while (Player.List.Count(r => r.IsHuman) > 0 && Player.List.Count(r => r.IsScp) > 0 && EventTime.TotalSeconds > 0)
            {
                Extensions.Broadcast($"<color=#D71868><b><i>Зомби Выживание</i></b></color>\n" +
                $"<color=yellow>Осталось людей: <color=green>{Player.List.Count(r => r.IsHuman)}</color></color>\n" +
                $"<color=yellow>Время до конца: <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            if (Player.List.Count(r => r.IsHuman) == 0)
            {
                Extensions.Broadcast($"<color=red>Зомби Победили!</color>\n" +
                $"<color=yellow>Зомби всех заразили</color>", 10);
            }
            else if (Player.List.Count(r => r.IsScp) == 0)
            {
                Extensions.Broadcast($"<color=yellow><color=#D71868><b><i>Люди</i></b></color> Победили!</color>\n" +
                $"<color=yellow>Люди остановили чуму и убили всех зомби</color>", 10);
            }
            else
            {
                Extensions.Broadcast($"<color=yellow><color=#D71868><b><i>Люди</i></b></color> Победили!</color>\n" +
                $"<color=yellow>Люди выжили, но это ещё не конец</color>", 10);
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
