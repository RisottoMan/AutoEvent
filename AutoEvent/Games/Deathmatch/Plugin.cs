using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Deathmatch
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = "Team Death-Match";
        public override string Description { get; set; } = "Team Death-Match on the Shipment map from MW19";
        public override string Author { get; set; } = "RisottoMan/code & xleb.ik/map";
        public override string CommandName { get; set; } = "tdm";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "Shipment", 
            Position = new Vector3(93f, 1020f, -43f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "ClassicMusic.ogg", 
            Volume = 5
        };
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler _eventHandler { get; set; }
        internal int MtfKills { get; set; }
        internal int ChaosKills { get; set; }
        private int _needKills;
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDying += _eventHandler.OnPlayerDying;
            Players.HandCuff += _eventHandler.OnHandCuff;
        }
        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PlayerDying -= _eventHandler.OnPlayerDying;
            Players.HandCuff -= _eventHandler.OnHandCuff;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            MtfKills = 0;
            ChaosKills = 0;
            _needKills = Config.KillsPerPerson * Player.GetPlayers().Count;

            float scale = 1;
            switch (Player.GetPlayers().Count())
            {
                case int n when (n > 20 && n <= 25): scale = 1.1f; break;
                case int n when (n > 25 && n <= 30): scale = 1.2f; break;
                case int n when (n > 35): scale = 1.3f; break;
            }

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

                Timing.CallDelayed(.1f, () =>
                {
                    if (item != null)
                    {
                        player.CurrentItem = item;
                    }
                });
            }
        }

        protected override bool IsRoundDone()
        {
            return !(MtfKills < _needKills && ChaosKills < _needKills && 
                Player.GetPlayers().Count(r => r.IsNTF) > 0 &&
                Player.GetPlayers().Count(r => r.IsChaos) > 0);
        }

        protected override void ProcessFrame()
        {
            string mtfColor = "<color=#42AAFF>";
            string chaosColor = "<color=green>";
            string whiteColor = "<color=white>";
            int mtfIndex = mtfColor.Length + (int)((float)MtfKills / _needKills * 20f);
            int chaosIndex = whiteColor.Length + 20 - (int)((float)ChaosKills / _needKills * 20f);
            string mtfString = $"{mtfColor}||||||||||||||||||||{mtfColor}".Insert(mtfIndex, whiteColor);
            string chaosString = $"{whiteColor}||||||||||||||||||||".Insert(chaosIndex, chaosColor);

            Extensions.Broadcast(
                Translation.Cycle.Replace("{name}", Name).Replace("{mtftext}", $"{MtfKills} {mtfString}")
                    .Replace("{chaostext}", $"{chaosString} {ChaosKills}"), 1);

        }

        protected override void OnFinished()
        {
            if (MtfKills == _needKills)
            {
                Extensions.Broadcast(Translation.MtfWin.Replace("{name}", Name), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.ChaosWin.Replace("{name}", Name), 10);
            }        
        }

    }
}
