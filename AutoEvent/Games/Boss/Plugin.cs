using MER.Lite.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Boss
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossCommandName;
        public MapInfo MapInfo { get; set; } = new MapInfo() 
            { MapName = "DeathParty", Position = new Vector3(6f, 1030f, -43.5f) };

        public SoundInfo SoundInfo { get; set; } = new SoundInfo() 
            { SoundName = "Boss.ogg", Loop = true, Volume = 7, StartAutomatically = false };

        [EventConfig] 
        public BossConfig Config { get; set; } 
        private EventHandler EventHandler { get; set; }
        private BossTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.BossTranslate;
        private List<Player> _boss;
        private TimeSpan _elapsedDuration { get; set; }
        private int _maxHp { get; set; }

        protected override void OnStart()
        {
            _elapsedDuration = TimeSpan.FromSeconds(Config.DurationInSeconds);
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.Loadouts);
                // Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
                // player.Health = 200;

                // RandomClass.CreateSoldier(player);
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = player.Items.First(); });
            }

        }
        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler();
            
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
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

            EventHandler = null;
        }


        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.BossTimeLeft.Replace("{time}", $"{time}"), 5);
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }

        protected override bool IsRoundDone()
        {
            // Round Time is shorter than 2 minutes (+ 15 seconds for countdown)
            return !(EventTime.TotalSeconds < Config.DurationInSeconds 
                   && EndConditions.TeamHasMoreThanXPlayers(Team.FoundationForces,0) 
                   && EndConditions.TeamHasMoreThanXPlayers(Team.ChaosInsurgency,0));
        }

        protected override void CountdownFinished()
        {
            _boss = new List<Player>();
            StartAudio();
            foreach (var player in Config.BossCount.GetPlayers())
            {
                _boss.Add(player);
                player.GiveLoadout(Config.BossLoadouts);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
                player.Health = Player.GetPlayers().Count() * 4000;
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = player.Items.First(); });
            }

            _maxHp = (int)_boss.Sum(x => x.Health);
            TimeSpan duration = EventTime.Subtract(TimeSpan.FromSeconds(Config.DurationInSeconds));
        }
        
        protected override void ProcessFrame()
        {
            string hpBar = ">■";
            for (int i = 0; i < (int)((_boss.Sum(x => x.Health) * 10) / _maxHp); i++)
            {
                hpBar += "■■";
            }
            hpBar += "<";

            string text = Translation.BossCounter;
            text = text.Replace("{hpbar}", hpBar);
            text = text.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsNTF)}");
            text = text.Replace("{time}", $"{_elapsedDuration.Minutes:00}:{_elapsedDuration.Seconds:00}");

            Extensions.Broadcast(text, 1);

            _elapsedDuration -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.BossWin.Replace("{hp}", $"{(int)_boss.Sum(x => x.Health)}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.BossHumansWin.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsNTF)}"), 10);
            }
        }
    }
}
