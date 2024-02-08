using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Boss
{
    public class Plugin : Event, IEventMap, IInternalEvent, IEventTag, IHidden
    {
        public override string Name { get; set; } = "Boss Battle";
        public override string Description { get; set; } = "Kill the Boss to win";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "boss";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo() 
        { 
            MapName = "Boss",
            Position = new Vector3(6f, 1030f, -43.5f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo() 
        { 
            SoundName = "SantaBegin.ogg", 
            Loop = false,
            Volume = 7, 
            StartAutomatically = true
        };
        public TagInfo TagInfo { get; set; } = new TagInfo()
        {
            Name = "Reworked",
            Color = "#77dde7"
        };
        private EventHandler _eventHandler;
        private IBossState _curState;
        private IBossState _prevState;
        private int _stage;
        public GameObject santaObject;
        public GameObject radiusArena;
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
            _stage = 0;
            foreach(GameObject block in MapInfo.Map.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "SantaClaus": santaObject = block; break;
                    case "Arena": radiusArena = block; break;
                }
            }
            
            foreach (Player player in Player.GetPlayers())
            {
                //player.GiveLoadout(Config.Loadouts);
                player.Position = RandomClass.GetRandomSpawnPosition(MapInfo.Map);
                //player.Health = 200;

                Timing.CallDelayed(0.1f, () => { player.CurrentItem = player.Items.First(); });
            }
        }
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            //StartAudio(true);
            // Санта летит на санях и падает в центр карты анимацией, "охохохо я вижу плохих игроков"
            /*
            while (SoundInfo.AudioPlayerBase.isActiveAndEnabled)
            {
                DebugLogger.LogDebug("cycle isActiveAndEnabled");
                // Extensions.Broadcast(Translation.BossTimeLeft.Replace("{time}", $"{time}"), 5);
                yield return Timing.WaitForSeconds(1f);
            }
            yield return Timing.WaitForSeconds(1f);
            */
            // После этого начинается бесконечный цикл и пока санта не скажет от 5 до 0 игра не начнется "Five Four Three Two One"
            // В момент, когда санта скажет 5, всем игрокам выдастся оружие в руки.
            //while (SoundInfo.IsEnded)

            yield break;
        }
        protected override void CountdownFinished()
        {
            // Санте выдаем первичное состояние, он прождет 15 секунд, после чего начнет двигаться?
            santaObject.transform.position = MapInfo.Map.Position;
            //StartAudio();
        }
        protected override bool IsRoundDone()
        {
            return !(EventTime.TotalSeconds < Config.DurationInSeconds);
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            string text = Translation.Counter;
            text = text.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsNTF)}");
            text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");
            Extensions.Broadcast(text, 1);

            StateUpdate();
        }

        protected void StateUpdate()
        {
            if (_curState is null)
            {
                _curState = _prevState is not WaitingState ? new WaitingState() : new FunnyMessageState();// : new RunningState();// : Functions.GetRandomState(_stage);
                _curState.Init(this);
                DebugLogger.LogDebug(_curState.Name);
            }

            _curState.Update();

            if (_curState.Timer.TotalSeconds > 0)
            {
                _curState.Timer -= TimeSpan.FromSeconds(FrameDelayInSeconds);
            }
            else
            {
                _curState.Deinit();
                _prevState = _curState;
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
                Extensions.Broadcast(Translation.HumansWin.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsNTF)}"), 10);
            }
        }
    }
}
