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
    public class Plugin// : IEvent
    {
        public string Name => AutoEvent.Singleton.Translation.DeathmatchName;
        public string Description => AutoEvent.Singleton.Translation.DeathmatchDescription;
        public string Color => "FFFF00";
        public string CommandName => "deathmatch";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public List<Vector3> Spawners { get; set; } = new List<Vector3>();

        public int MtfKills;
        public int ChaosKills;
        public int NeedKills;
        EventHandler _eventHandler;

        public void OnStart()
        {
            _eventHandler = new EventHandler(this);

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned += _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole += _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.Dying += _eventHandler.OnDying;
            Exiled.Events.Handlers.Player.Shooting += _eventHandler.OnShooting;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Player.Spawned -= _eventHandler.OnSpawned;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= _eventHandler.OnPlaceBullet;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnDying;
            Exiled.Events.Handlers.Player.Shooting -= _eventHandler.OnShooting;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;

            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("Shipment", new Vector3(120f, 1020f, -43.5f), new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1));
            //Extensions.PlayAudio("ClassicMusic.ogg", 3, true, Name);

            MtfKills = 0;
            ChaosKills = 0;
            // Choosing the number of kills for the end of the mini-game
            for (int i = 0; i < Player.List.Count(); i += 5)
            {
                NeedKills += 15;
            }
            var count = 0;
            foreach (Player player in Player.List)
            {
                if (count % 2 == 0)
                {
                    player.Role.Set(RoleTypeId.NtfSergeant);
                    player.CurrentItem = player.Items.ElementAt(1);
                    player.Position = GameMap.Position + RandomClass.GetRandomPosition();
                }
                else
                {
                    player.Role.Set(RoleTypeId.ChaosRifleman);
                    player.CurrentItem = player.Items.ElementAt(1);
                    player.Position = GameMap.Position + RandomClass.GetRandomPosition();
                }
                player.EnableEffect<CustomPlayerEffects.Scp1853>(300);
                player.EnableEffect(EffectType.MovementBoost, 300);
                player.ChangeEffectIntensity(EffectType.MovementBoost, 25);
                player.EnableEffect<CustomPlayerEffects.SpawnProtected>(10);
                count++;
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
            // If you need to stop the game, then just kill all the players
            while (MtfKills < NeedKills && ChaosKills < NeedKills && Player.List.Count(r => r.IsAlive) > 0)
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
        }
    }
}
