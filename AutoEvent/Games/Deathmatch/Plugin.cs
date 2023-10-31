using CustomPlayerEffects;
using MER.Lite.Objects;
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
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.DeathmatchTranslate.DeathmatchCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public DeathmatchConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Shipment", Position = new Vector3(93f, 1020f, -43f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Ultrakill.ogg", Volume = 10, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private DeathmatchTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.DeathmatchTranslate;
        public int MtfKills { get; set; }
        public int ChaosKills { get; set; }
        private int _needKills;
        private bool isFriendlyFire { get; set; }

        protected override void RegisterEvents()
        {
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

            isFriendlyFire = Server.FriendlyFire;
        }
        protected override void UnregisterEvents()
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
            Server.FriendlyFire = isFriendlyFire;
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
                if (UnityEngine.Random.Range(0,2) == 1)
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
            // Mtf team doesnt have enough kills (NeedKills) &&
            // Chaos team doesnt have enough kills (NeedKills) &&
            // Both teams have at least one player
            return !(MtfKills < _needKills && ChaosKills < _needKills && Player.GetPlayers().Count(r => r.IsNTF) > 0 &&
                   Player.GetPlayers().Count(r => r.IsChaos) > 0);
        }

        protected override void ProcessFrame()
        {
            //string mtfString = string.Empty;
            //string chaosString = string.Empty;
            
            // This wont crash the server because its not possible to enumerate into 0.
            // These colors allow us to change the color of the mtf percent bar without
            // affecting the index offset to compensate for the spare characters.
            string mtfColor = "<color=#42AAFF>";
            string chaosColor = "<color=green>";
            string whiteColor = "<color=white>";
            int mtfIndex = mtfColor.Length + (int)((float)MtfKills / _needKills * 20f);
            int chaosIndex = whiteColor.Length + 20 - (int)((float)ChaosKills / _needKills * 20f);
            string mtfString = $"{mtfColor}||||||||||||||||||||{mtfColor}".Insert(mtfIndex, whiteColor);
            string chaosString = $"{whiteColor}||||||||||||||||||||".Insert(chaosIndex, chaosColor);
            
            /*for (int i = 0; i < neededKills; i += (int)(_needKills / 5))
            {
                if (MtfKills >= i) 
                    mtfString += "■";
                else 
                    mtfString += "□";

                if (ChaosKills >= i) 
                    chaosString = "■" + chaosString;
                else 
                    chaosString = "□" + chaosString;
            }*/

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

    }
}
