using CustomPlayerEffects;
using AutoEvent.API.Schematic.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Handlers;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Deathmatch
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.DeathmatchName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.DeathmatchDescription;
        public override string Author { get; set; } = "KoT0XleB";
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

            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDying += _eventHandler.OnPlayerDying;
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
            Players.PlayerDying -= _eventHandler.OnPlayerDying;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            float scale = 1;
            switch (Player.GetPlayers().Count())
            {
                case int n when (n > 20 && n <= 25): scale = 1.1f; break;
                case int n when (n > 25 && n <= 30): scale = 1.2f; break;
                case int n when (n > 35): scale = 1.3f; break;
            }

            GameMap = Extensions.LoadMap(MapName, new Vector3(93f, 1020f, -43f), Quaternion.Euler(Vector3.zero), Vector3.one * scale);
            Extensions.PlayAudio("ClassicMusic.ogg", 3, true, Name);

            MtfKills = 0;
            ChaosKills = 0;

            for (int i = 0; i < Player.GetPlayers().Count(); i += 5)
            {
                NeedKills += 15;
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetRandomPosition(GameMap);
                }
                else
                {
                    Extensions.SetRole(player, RoleTypeId.ChaosRifleman, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetRandomPosition(GameMap);
                }
                count++;

                player.EffectsManager.EnableEffect<Scp1853>(300);
                player.EffectsManager.EnableEffect<MovementBoost>(300);
                player.EffectsManager.ChangeState<MovementBoost>(10);
            }

            Timing.RunCoroutine(OnEventRunning(), "deathmatch_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            foreach(Player player in Player.GetPlayers())
            {
                player.AddItem(RandomClass.RandomItems.RandomItem());
                player.AddItem(ItemType.ArmorCombat);

                Timing.CallDelayed(0.2f, () =>
                {
                    player.CurrentItem = player.Items.ElementAt(0);
                });
            }

            while (MtfKills < NeedKills && ChaosKills < NeedKills && Player.GetPlayers().Count(r => r.IsNTF) > 0 && Player.GetPlayers().Count(r => r.IsChaos) > 0)
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

                Extensions.Broadcast(translation.DeathmatchCycle.
                    Replace("{name}", Name).
                    Replace("{mtftext}", $"{MtfKills} {mtfString}").
                    Replace("{chaostext}", $"{chaosString} {ChaosKills}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            if (MtfKills == NeedKills)
            {
                Extensions.Broadcast(translation.DeathmatchMtfWin.Replace("{name}", Name), 10);
            }
            else
            {
                Extensions.Broadcast(translation.DeathmatchChaosWin.Replace("{name}", Name), 10);
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
