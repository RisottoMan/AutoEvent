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

namespace AutoEvent.Games.AllDeathmatch
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.AllTranslation.AllName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.AllTranslation.AllDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.AllTranslation.AllCommandName;
        private AllTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.AllTranslation;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        private EventHandler EventHandler { get; set; }
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
        [EventConfig]
        public Config Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "de_dust2",
            Position = new Vector3(0, 0, 30)
        };

        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "ExecDeathmatch.ogg", 
            Volume = 10,
            Loop = true 
        };
        public List<GameObject> Spawnpoints { get; set; }
        public int NeededKills { get; set; }
        public Dictionary<Player, int> TotalKills { get; set; }
        public Player Winner { get; set; }
        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler(this);

            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropAmmo += EventHandler.OnDropAmmo;
            Players.PlayerDying += EventHandler.OnPlayerDying;
        }
        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropAmmo -= EventHandler.OnDropAmmo;
            Players.PlayerDying -= EventHandler.OnPlayerDying;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            Winner = null;
            NeededKills = 0;
            switch (Player.GetPlayers().Count())
            {
                case int n when (n > 0 && n <= 5): NeededKills = 5; break;
                case int n when (n > 5 && n <= 10): NeededKills = 15; break;
                case int n when (n > 10 && n <= 20): NeededKills = 25; break;
                case int n when (n > 20 && n <= 25): NeededKills = 50; break;
                case int n when (n > 25 && n <= 35): NeededKills = 75; break;
                case int n when (n > 35): NeededKills = 100; break;
            }

            TotalKills = new();
            Spawnpoints = RandomClass.GetAllSpawnpoint(MapInfo.Map);
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.NTFLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
                player.Position = Spawnpoints.RandomItem().transform.position;

                TotalKills.Add(player, 0);
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
            return !(Config.TimeMinutesRound >= EventTime.TotalMinutes && 
               Player.GetPlayers().Count(r => r.IsAlive) > 0 && 
               Winner is null);
        }

        protected override void ProcessFrame()
        {
            double remainTime = Config.TimeMinutesRound - EventTime.TotalMinutes;
            var time = $"{(int)remainTime:00}:{(int)((remainTime * 60) % 60):00}";
            var sortedDict = TotalKills.OrderByDescending(r => r.Value).ToDictionary(x => x.Key, x => x.Value);

            int playerCount = Player.GetPlayers().Count();
            //string pos = "20em";
            //string vset = "10em";
            foreach(Player player in Player.GetPlayers())
            {
                // LEADERBOARD FEATURE, but it dosnt work :(
                /*
                var text = $"<size=30><pos={pos}><voffset={vset}><i>Leaderboard:</i>\n";
                if (playerCount >= 3)
                {
                    text += $"<voffset={vset}><color=#ffd700>1. {sortedDict.ElementAt(0).Key.Nickname} / {sortedDict.ElementAt(0).Value} kills</color>\n" +
                    $"<voffset={vset}><color=#c0c0c0>2. {sortedDict.ElementAt(1).Key.Nickname} / {sortedDict.ElementAt(1).Value} kills</color>\n" +
                    $"<voffset={vset}><color=#cd7f32>3. {sortedDict.ElementAt(2).Key.Nickname} / {sortedDict.ElementAt(2).Value} kills</color>\n";
                }

                var playerItem = sortedDict.First(x => x.Key == player);
                text += $"<voffset={vset}><color=#ff0000>You - {playerItem.Value} kills</color></size>";
                player.ReceiveHint(text, 1);
                */

                if (TotalKills[player] >= NeededKills)
                {
                    Winner = player;
                }

                var playerItem = sortedDict.FirstOrDefault(x => x.Key == player);
                string text = AutoEvent.Singleton.Translation.AllTranslation.AllCycle.
                    Replace("{name}", Name).
                    Replace("{kills}", playerItem.Value.ToString()).
                    Replace("{needKills}", NeededKills.ToString()).
                    Replace("{time}", time);

                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);
            }
        }

        protected override void OnFinished()
        {
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            foreach (Player player in Player.GetPlayers())
            {
                string text = string.Empty;
                if (Player.GetPlayers().Count(r => r.IsAlive) == 0)
                {
                    text = Translation.AllNoPlayers;
                }
                else if (EventTime.TotalMinutes >= Config.TimeMinutesRound)
                {
                    text = Translation.AllTimeEnd;
                }
                else if (Winner != null)
                {
                    text = Translation.AllWinnerEnd.
                        Replace("{winner}", Winner.Nickname).
                        Replace("{time}", time);
                }
                else
                {
                     //var maxKill = TotalKills.OrderByDescending(x => x.Value).FirstOrDefault();
                }

                text = text.Replace("{count}", TotalKills.First(x => x.Key == player).Value.ToString());
                player.ClearBroadcasts();
                player.SendBroadcast(text, 10);
            }
        }
    }
}