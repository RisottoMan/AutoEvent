using MEC;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.HideAndSeek
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Tag";
        public override string Description { get; set; } = "We need to catch up with all the players on the map";
        public override string Author { get; set; } = "RisottoMan";
        public override string CommandName { get; set; } = "tag";
        public override Version Version { get; set; } = new Version(1, 1, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "HideAndSeek", 
            Position = new Vector3(0, 30, 30)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "HideAndSeek.ogg", 
            Volume = 5
        };
        private EventHandler _eventHandler;
        private TimeSpan _countdown;
        private EventState _eventState;
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
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;
            Players.ChargingJailbird += _eventHandler.OnJailbirdCharge;
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
            Players.PlayerDamage -= _eventHandler.OnPlayerDamage;
            Players.ChargingJailbird -= _eventHandler.OnJailbirdCharge;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _eventState = 0;
            List<GameObject> spawnpoints = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList();
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = spawnpoints.RandomItem().transform.position;
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float _time = 15; _time > 0; _time--)
            {
                Extensions.Broadcast(Translation.Broadcast.Replace("{time}", $"{_time}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
        }

        protected override bool IsRoundDone()
        {
            _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
            return !(Player.GetPlayers().Count(ply => ply.IsAlive) > 1);
        }

        protected override void ProcessFrame()
        {
            string text = string.Empty;
            switch (_eventState)
            {
                case EventState.SelectPlayers: SelectPlayers(ref text); break;
                case EventState.TagPeriod: UpdateTagPeriod(ref text); break;
                case EventState.KillTaggers: KillTaggers(ref text); break;
                case EventState.PlayerBreak: UpdatePlayerBreak(ref text); break;
            }

            Extensions.Broadcast(text, 1);
        }

        /// <summary>
        /// Choosing the player(s) who will catch up with other players
        /// </summary>
        protected void SelectPlayers(ref string text)
        {
            text = Translation.Broadcast.Replace("{time}", $"{_countdown.TotalSeconds}");
            List<Player> playersToChoose = Player.GetPlayers().Where(x => x.IsAlive).ToList();
            foreach (Player ply in Config.TaggerCount.GetPlayers(true, playersToChoose))
            {
                ply.GiveLoadout(Config.TaggerLoadouts);
                var item = ply.AddItem(Config.TaggerWeapon);
                Timing.CallDelayed(0.1f, () =>
                {
                    if (item != null)
                    {
                        ply.CurrentItem = item;
                    }
                });
            }

            if (Player.GetPlayers().Count(ply => ply.HasLoadout(Config.PlayerLoadouts)) <= Config.PlayersRequiredForBreachScannerEffect)
            {
                foreach (Player ply in Player.GetPlayers().Where(ply => ply.HasLoadout(Config.PlayerLoadouts)))
                {
                    ply.GiveEffect(StatusEffect.Scanned, 255, 0f, false);
                }
            }

            _countdown = new TimeSpan(0, 0, Config.TagDuration);
            _eventState++;
        }

        /// <summary>
        /// Just waiting N seconds until the time runs out
        /// </summary>
        protected void UpdateTagPeriod(ref string text)
        {
            text = Translation.Cycle.Replace("{time}", $"{_countdown.TotalSeconds}");

            if (_countdown.TotalSeconds <= 0)
                _eventState++;
        }

        /// <summary>
        /// Kill players who are taggers.
        /// </summary>
        protected void KillTaggers(ref string text)
        {
            text = Translation.Cycle.Replace("{time}", $"{_countdown.TotalSeconds}");

            foreach (Player player in Player.GetPlayers())
            {
                if (player.Items.Any(r => r.ItemTypeId == Config.TaggerWeapon))
                {
                    player.ClearInventory();
                    player.Damage(200, Translation.Hurt);
                }
            }

            _countdown = new TimeSpan(0, 0, Config.BreakDuration);
            _eventState++;
        }

        /// <summary>
        /// Wait for N seconds before choosing next batch.
        /// </summary>
        protected void UpdatePlayerBreak(ref string text)
        {
            text = Translation.Broadcast.Replace("{time}", $"{_countdown.TotalSeconds}");

            if (_countdown.TotalSeconds <= 0)
                _eventState = 0;
        }

        protected override void OnFinished()
        {
            string text = string.Empty;
            if (Player.GetPlayers().Count(r => r.IsAlive) >= 1)
            {
                text = Translation.OnePlayer
                    .Replace("{winner}", Player.GetPlayers().First(r => r.IsAlive).Nickname)
                    .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");
            }
            else
            {
                text = Translation.AllDie
                    .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");
            }

            Extensions.Broadcast(text, 10);
        }
    }
}
