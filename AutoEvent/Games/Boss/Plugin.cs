using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;
using AutoEvent.Games.Boss.Features;

namespace AutoEvent.Games.Boss
{
    public class Plugin : Event, IEventMap, IInternalEvent, IEventTag//, IHidden
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public BossConfig Config { get; set; }
        private BossTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.BossTranslate;
        public MapInfo MapInfo { get; set; } = new MapInfo() 
        { 
            MapName = "Boss_Santa",
            Position = new Vector3(6f, 1030f, -43.5f) 
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo() 
        { 
            SoundName = "SantaMusic.ogg", 
            Loop = true, Volume = 7, 
            StartAutomatically = false 
        };
        public TagInfo TagInfo { get; set; } = new TagInfo()
        {
            Name = "Reworked",
            Color = "#77dde7"
        };
        private EventHandler _eventHandler;
        private List<Type> _eventStates;
        private IBossState _curState;
        private IBossState _previousState;
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
            Players.PlayerDamage += _eventHandler.OnDamage;
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
            Players.PlayerDamage -= _eventHandler.OnDamage;

            _eventHandler = null;
        }
        protected override void OnStart()
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.Loadouts);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
                player.Health = 200;

                Timing.CallDelayed(0.1f, () => { player.CurrentItem = player.Items.First(); });
            }

        }
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(Translation.BossTimeLeft.Replace("{time}", $"{time}"), 5);
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }

        protected override bool IsRoundDone()
        {
            return !(EventTime.TotalSeconds < Config.DurationInSeconds);
        }

        protected override void CountdownFinished()
        {
            StartAudio();
        }

        protected override void ProcessFrame()
        {
            string text = Translation.BossCounter;
            text = text.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsNTF)}");
            text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");
            Extensions.Broadcast(text, 1);

            if (_curState is null)
            {
                _curState = new RunningState();
                _curState = _previousState is not WaitingState ? new WaitingState() : Functions.GetRandomState(_previousState);

                DebugLogger.LogDebug($"New state {_curState.Name}");

                _curState.Init();
            }

            _curState.Update();

            if (_curState.Timer.TotalSeconds > 0)
            {
                _curState.Timer -= TimeSpan.FromSeconds(FrameDelayInSeconds);
            }
            else
            {
                _curState.Deinit();
                _previousState = _curState;
                _curState = null;
            }
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.BossWin, 10);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.BossHumansWin.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsNTF)}"), 10);
            }
        }
    }
}
