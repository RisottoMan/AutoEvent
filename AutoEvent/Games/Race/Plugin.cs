using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Race
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Race";
        public override string Description { get; set; } = "Get to the end of the map to win";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "race";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "Race", 
            Position = new Vector3(115.5f, 1030f, -43.5f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "FinishWay.ogg", 
            Volume = 8, 
            Loop = false, 
            StartAutomatically = false
        };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler _eventHandler { get; set; }
        private TimeSpan _remainingTime;
        private GameObject _point;

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
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
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.Loadouts);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<b>{time}</b>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            _remainingTime = new TimeSpan(0, 0, Config.EventDurationInSeconds);
            StartAudio();
            _point = new GameObject();
            foreach(var gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Wall": { GameObject.Destroy(gameObject); } break;
                    case "Lava": { gameObject.AddComponent<LavaComponent>(); } break;
                    case "FinishTrigger": { _point = gameObject; } break;
                }
            }
        }

        protected override bool IsRoundDone()
        {
            // At least one player is alive &&
            // Elapsed time is shorter than a minute (+ broadcast duration)
            return !(Player.GetPlayers().Count(r => r.IsAlive) > 0 && EventTime.TotalSeconds < Config.EventDurationInSeconds );
        }

        protected override void ProcessFrame()
        {

            var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
            var time = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";

            Extensions.Broadcast(Translation.Cycle.Replace("{name}", Name).Replace("{time}", time), 1);
            _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        }

        protected override void OnFinished()
        {
            foreach(Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _point.transform.position) > 10)
                {
                    player.Kill(Translation.Died);
                }
            }

            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(Translation.PlayersSurvived.Replace("{count}", Player.GetPlayers().Count(r => r.IsAlive).ToString()), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.OneSurvived.Replace("{player}", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.NoSurvivors, 10);
            }
        }
    }
}
