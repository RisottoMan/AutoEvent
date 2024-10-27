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
using System.Text;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.AllDeathmatch
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = "All Deathmatch";
        public override string Description { get; set; } = "Fight against each other in all deathmatch.";
        public override string Author { get; set; } = "RisottoMan";
        public override string CommandName { get; set; } = "dm";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "de_dust2",
            Position = new Vector3(0, 30, 30)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "ExecDeathmatch.ogg", 
            Volume = 10
        };
        private EventHandler _eventHandler { get; set; }
        internal List<GameObject> Points { get; set; }
        private int _needKills { get; set; }
        internal Dictionary<Player, int> TotalKills { get; set; }
        private Player _winner { get; set; }
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);

            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDying += _eventHandler.OnPlayerDying;
        }
        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PlayerDying -= _eventHandler.OnPlayerDying;

            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _winner = null;
            _needKills = 0;
            TotalKills = new();

            switch (Player.GetPlayers().Count())
            {
                case int n when (n > 0 && n <= 5): _needKills = 10; break;
                case int n when (n > 5 && n <= 10): _needKills = 15; break;
                case int n when (n > 10 && n <= 20): _needKills = 25; break;
                case int n when (n > 20 && n <= 25): _needKills = 50; break;
                case int n when (n > 25 && n <= 35): _needKills = 75; break;
                case int n when (n > 35): _needKills = 100; break;
            }

            Points = RandomClass.GetAllSpawnpoint(MapInfo.Map);
            // Walls can be used on all counter-strike maps. For Deathmatch mode, they must be removed earlier.
            MapInfo.Map.AttachedBlocks.Where(r => r.name == "Wall").ToList()
                .ForEach(r => GameObject.Destroy(r));

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.NTFLoadouts, LoadoutFlags.ForceInfiniteAmmo | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
                player.Position = Points.RandomItem().transform.position;

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
               Player.GetPlayers().Count(r => r.IsAlive) > 1 && 
               _winner is null);
        }

        protected override void ProcessFrame()
        {
            double remainTime = Config.TimeMinutesRound - EventTime.TotalMinutes;
            var time = $"{(int)remainTime:00}:{(int)((remainTime * 60) % 60):00}";
            var sortedDict = TotalKills.OrderByDescending(r => r.Value).ToDictionary(x => x.Key, x => x.Value);

            StringBuilder leaderboard = new StringBuilder("Leaderboard:\n");
            for (int i = 0; i < 3; i++)
            {
                if (i < sortedDict.Count)
                {
                    string color = string.Empty;
                    switch (i)
                    {
                        case 0: color = "#ffd700"; break;
                        case 1: color = "#c0c0c0"; break;
                        case 2: color = "#cd7f32"; break;
                    }

                    int length = Math.Min(sortedDict.ElementAt(i).Key.Nickname.Length, 10);
                    leaderboard.Append($"<color={color}>{i + 1}. ");
                    leaderboard.Append($"{sortedDict.ElementAt(i).Key.Nickname.Substring(0, length)} ");
                    leaderboard.Append($"/ {sortedDict.ElementAt(i).Value} kills</color>\n");
                }
            }

            foreach (Player player in Player.GetPlayers())
            {
                string playerText = string.Empty;

                if (TotalKills[player] >= _needKills)
                {
                    _winner = player;
                }

                var playerItem = sortedDict.FirstOrDefault(x => x.Key == player);
                playerText = leaderboard + $"<color=#ff0000>You - {playerItem.Value}/{_needKills} kills</color></size>";

                string text = Translation.Cycle.
                    Replace("{name}", Name).
                    Replace("{kills}", playerItem.Value.ToString()).
                    Replace("{needKills}", _needKills.ToString()).
                    Replace("{time}", time);

                player.ReceiveHint($"<line-height=95%><voffset=25em><align=right><size=30>{playerText}</size></align>", 1);
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
                    text = Translation.NoPlayers;
                }
                else if (EventTime.TotalMinutes >= Config.TimeMinutesRound)
                {
                    text = Translation.TimeEnd;
                }
                else if (_winner != null)
                {
                    text = Translation.WinnerEnd.Replace("{winner}", _winner.Nickname).Replace("{time}", time);
                }

                text = text.Replace("{count}", TotalKills.First(x => x.Key == player).Value.ToString());
                player.ClearBroadcasts();
                player.SendBroadcast(text, 10);
            }
        }
    }
}