using CustomPlayerEffects;
using AutoEvent.API.Schematic.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Deathmatch
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.DeathmatchTranslate.DeathmatchName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.DeathmatchTranslate.DeathmatchDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "deathmatch";
        [EventConfig]
        public DeathmatchConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Shipment", Position = new Vector3(93f, 1020f, -43f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "ClassicMusic.ogg", Volume = 3, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private DeathmatchTranslate Translation { get; set; }
        public int MtfKills { get; set; }
        public int ChaosKills { get; set; }
        private int _needKills;

        protected override void RegisterEvents()
        {
            Translation = new DeathmatchTranslate();
            EventHandler = new EventHandler(this);

            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
            Players.PlayerDying += EventHandler.OnPlayerDying;
            Players.HandCuff += EventHandler.OnHandCuff;
        }

        protected override void OnStart()
        {
            Server.FriendlyFire = false;
            float scale = 1;
            switch (Player.GetPlayers().Count())
            {
                case int n when (n > 20 && n <= 25): scale = 1.1f; break;
                case int n when (n > 25 && n <= 30): scale = 1.2f; break;
                case int n when (n > 35): scale = 1.3f; break;
            }


            MtfKills = 0;
            ChaosKills = 0;


            _needKills = Config.KillsPerPerson * Player.GetPlayers().Count;

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.GiveLoadout(Config.NTFLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
                    player.Position = RandomClass.GetRandomPosition(MapInfo.Map);
                }
                else
                {
                    player.GiveLoadout(Config.ChaosLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
                    player.Position = RandomClass.GetRandomPosition(MapInfo.Map);
                }
                count++;
            }
        }

        protected override void OnStop()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropItem -= EventHandler.OnDropItem;
            Players.DropAmmo -= EventHandler.OnDropAmmo;
            Players.PlayerDying -= EventHandler.OnPlayerDying;
            Players.HandCuff -= EventHandler.OnHandCuff;

            EventHandler = null;
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            foreach(Player player in Player.GetPlayers())
            {
                var item = player.AddItem(Config.AvailableWeapons.RandomItem());
                player.AddItem(ItemType.ArmorCombat);

                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = item;
                });
            }
        }

        protected override bool IsRoundDone()
        {
            // Mtf team doesnt have enough kills (NeedKills) &&
            // Chaos team doesnt have enough kills (NeedKills) &&
            // Both teams have at least one player
            return !(MtfKills < _needKills && ChaosKills < _needKills && Player.GetPlayers().Count(r => r.IsNTF) > 0 &&
                   Player.GetPlayers().Count(r => r.IsChaos) > 0);
        }

        protected override void ProcessFrame()
        {
            string mtfString = string.Empty;
            string chaosString = string.Empty;
            for (int i = 0; i < _needKills; i += (int)(_needKills / 5))
            {
                if (MtfKills >= i) mtfString += "■";
                else mtfString += "□";

                if (ChaosKills >= i) chaosString = "■" + chaosString;
                else chaosString = "□" + chaosString;
            }

            Extensions.Broadcast(
                Translation.DeathmatchCycle.Replace("{name}", Name).Replace("{mtftext}", $"{MtfKills} {mtfString}")
                    .Replace("{chaostext}", $"{chaosString} {ChaosKills}"), 1);

        }

        protected override void OnFinished()
        {
            if (MtfKills == _needKills)
            {
                Extensions.Broadcast(Translation.DeathmatchMtfWin.Replace("{name}", Name), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.DeathmatchChaosWin.Replace("{name}", Name), 10);
            }        
        }

        protected override void OnCleanup()
        {
            Server.FriendlyFire = AutoEvent.IsFriendlyFireEnabledByDefault;
        }
    }
}
