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

namespace AutoEvent.Events.Deathmatch
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.DeathmatchName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.DeathmatchDescription;
        public override string MapName { get; set; } = "Shipment";
        public override string CommandName { get; set; } = "deathmatch";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public List<Vector3> Spawners { get; set; } = new List<Vector3>();

        public int MtfKills;
        public int ChaosKills;
        public int NeedKills;
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
            Exiled.Events.Handlers.Player.Dying += _eventHandler.OnDying;
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
            Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnDying;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= _eventHandler.OnReloading;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            float scale = 1;
            switch (Player.List.Count())
            {
                case int n when (n > 20 && n <= 25): scale = 1.1f; break;
                case int n when (n > 25 && n <= 30): scale = 1.2f; break;
                case int n when (n > 35): scale = 1.3f; break;
            }

            GameMap = Extensions.LoadMap(MapName, new Vector3(93f, 1020f, -43f), Quaternion.Euler(Vector3.zero), Vector3.one * scale);
            Extensions.PlayAudio("ClassicMusic.ogg", 3, true, Name);

            MtfKills = 0;
            ChaosKills = 0;

            for (int i = 0; i < Player.List.Count(); i += 5)
            {
                NeedKills += 15;
            }

            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfSergeant, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetRandomPosition(GameMap);
                }
                else
                {
                    player.Role.Set(RoleTypeId.ChaosRifleman, SpawnReason.None, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetRandomPosition(GameMap);
                }
                count++;

                player.EnableEffect<CustomPlayerEffects.Scp1853>(300);
                player.EnableEffect(EffectType.MovementBoost, 300);
                player.ChangeEffectIntensity(EffectType.MovementBoost, 10);
            }

            Timing.RunCoroutine(OnEventRunning(), "deathmatch_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            foreach(Player player in Player.List)
            {
                player.AddItem(RandomClass.RandomItems.RandomItem());
                player.AddItem(ItemType.ArmorCombat);

                Timing.CallDelayed(0.2f, () =>
                {
                    player.CurrentItem = player.Items.ElementAt(0);
                });
            }

            while (MtfKills < NeedKills && ChaosKills < NeedKills && Player.List.Count(r => r.IsNTF) > 0 && Player.List.Count(r => r.IsCHI) > 0)
            {
                string mtfString = string.Empty;
                string chaosString = string.Empty;
                for (int i = 0; i < NeedKills; i += (int)(NeedKills / 5))
                {
                    if (MtfKills >= i) mtfString += "■";
                    else mtfString += "□";

                    if (ChaosKills >= i) chaosString = "■" + chaosString;
                    else chaosString = "□" + chaosString;
                }

                Extensions.Broadcast(trans.DeathmatchCycle.Replace("{name}", Name).Replace("{mtftext}", $"{MtfKills} {mtfString}").Replace("{chaostext}", $"{chaosString} {ChaosKills}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            if (MtfKills == NeedKills)
            {
                Extensions.Broadcast(trans.DeathmatchMtfWin.Replace("{name}", Name), 10);
            }
            else
            {
                Extensions.Broadcast(trans.DeathmatchChaosWin.Replace("{name}", Name), 10);
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
