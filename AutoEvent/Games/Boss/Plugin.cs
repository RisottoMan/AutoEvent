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
using AutoEvent.Games.Boss.Features;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Boss
{
    public class Plugin : Event, IEventMap, IInternalEvent, IEventTag, IHidden
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
        private EventHandler _eventHandler { get; set; }
        private StateClass<object> _stateClass;
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
            if (_stateClass.Timer.TotalSeconds > 0)
                _stateClass.Timer -= TimeSpan.FromSeconds(FrameDelayInSeconds);

            return !(EventTime.TotalSeconds < Config.DurationInSeconds);
        }

        protected override void CountdownFinished()
        {
            _stateClass = new StateClass<object>()
            {
                SantaObject = Functions.CreateSchematicBoss(),
                CurrentState = StateEnum.Waiting,
                CurrentValue = new WaitingState(),
                Timer = new TimeSpan(0, 0, 5)
            };

            StartAudio();
        }

        protected override void ProcessFrame()
        {
            string text = Translation.BossCounter;
            text = text.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsNTF)}");
            text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");
            Extensions.Broadcast(text, 1);

            switch (_stateClass.CurrentState)
            {
                case StateEnum.Waiting:
                    HandleWaitingState(Translation, ref text);
                    break;
                case StateEnum.Running:
                    HandleRunningState(Translation, ref text);
                    break;
            }
        }

        protected void HandleWaitingState(BossTranslate trans, ref string text)
        {
            // Santa has to come running to the center for the next event
            if (Vector3.Distance(_stateClass.SantaObject.Position, MapInfo.Position) > 5)
            {
                _stateClass.SantaObject.Position = MapInfo.Position;
            }

            if (_stateClass.Timer.TotalSeconds <= 0)
            {
                //_stateClass.CurrentState = Functions.GetRandomState();
                return;
            }
        }

        protected void HandleRunningState(BossTranslate trans, ref string text)
        {
            if (Vector3.Distance(_stateClass.SantaObject.Position, MapInfo.Position) > 5)
            {
                _stateClass.SantaObject.Position = MapInfo.Position;
            }

            if (_stateClass.Timer.TotalSeconds <= 0)
            {
                _stateClass.CurrentState = StateEnum.Waiting;
                _stateClass.Timer = new TimeSpan(0, 0, 15);
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
