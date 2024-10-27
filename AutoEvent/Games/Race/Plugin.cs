using AutoEvent.Events.Handlers;
using MEC;
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
        public override string Author { get; set; } = "RisottoMan";
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
        private TimeSpan _countdown;
        private GameObject _finish;
        private GameObject _wall;
        internal GameObject Spawnpoint;

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
            Spawnpoint = new();
            _finish = new();
            _wall = new();
            _countdown = new TimeSpan(0, 0, Config.EventDurationInSeconds);

            foreach (var block in MapInfo.Map.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "Wall": _wall = block; break;
                    case "Lava": block.AddComponent<LavaComponent>().StartComponent(this); break;
                    case "Finish": _finish = block; break;
                    case "Spawnpoint": Spawnpoint = block; break;
                }
            }

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.Loadouts);
                player.Position = Spawnpoint.transform.position;
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
            StartAudio();
            GameObject.Destroy(_wall);
        }

        protected override bool IsRoundDone()
        {
            _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
            return !(Player.GetPlayers().Count(r => r.IsAlive) > 0 && EventTime.TotalSeconds < Config.EventDurationInSeconds );
        }

        protected override void ProcessFrame()
        {
            Extensions.Broadcast(Translation.Cycle.
                Replace("{name}", Name).
                Replace("{time}", $"{_countdown.Minutes:00}:{_countdown.Seconds:00}"), 1);
        }

        protected override void OnFinished()
        {
            foreach(Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _finish.transform.position) > 10)
                {
                    player.Kill(Translation.Died);
                }
            }

            string text = string.Empty;
            int count = Player.GetPlayers().Count(r => r.IsAlive);

            if (count > 1)
            {
                text = Translation.PlayersSurvived.Replace("{count}", Player.GetPlayers().Count(r => r.IsAlive).ToString());
            }
            else if (count == 1)
            {
                text = Translation.OneSurvived.Replace("{player}", Player.GetPlayers().First(r => r.IsAlive).Nickname);
            }
            else
            {
                text = Translation.NoSurvivors;
            }

            Extensions.Broadcast(text, 10);
        }
    }
}
