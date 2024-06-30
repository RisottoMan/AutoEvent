using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using AdminToys;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.MusicalChairs
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Musical Chairs";
        public override string Description { get; set; } = "Competition with other players for free chairs to funny music";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "chair";
        public override Version Version { get; set; } = new Version(1, 0, 4);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "MusicalChairs",
            Position = new Vector3(0, 0, 30),
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "MusicalChairs.ogg",
            Volume = 10,
            Loop = false
        };
        private EventHandler _eventHandler;
        private EventState _eventState;
        private GameObject _parentPlatform;
        internal List<GameObject> Platforms;
        private Dictionary<Player, PlayerClass> _playerDict;
        private TimeSpan _countdown;
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
            Players.UsingStamina += _eventHandler.OnUsingStamina;
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
            Players.UsingStamina -= _eventHandler.OnUsingStamina;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _eventState = 0;
            _countdown = new TimeSpan(0, 0, 5);
            List<GameObject> spawnpoints = new List<GameObject>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Spawnpoint": spawnpoints.Add(gameObject); break;
                    case "Cylinder-Parent": _parentPlatform = gameObject; break;
                }
            }

            int count = Player.GetPlayers().Count > 40 ? 40 : Player.GetPlayers().Count - 1;
            Platforms = Functions.GeneratePlatforms(count, _parentPlatform, MapInfo.Position);

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadout);
                player.Position = spawnpoints.RandomItem().transform.position;
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                string text = Translation.Start.Replace("{time}", time.ToString());
                Extensions.Broadcast(text, 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override bool IsRoundDone()
        {
            _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
            return !(Player.GetPlayers().Count(r => r.IsAlive) > 1);
        }
        protected override float FrameDelayInSeconds { get; set; } = 1f;
        protected override void ProcessFrame()
        {
            string text = string.Empty;
            switch (_eventState)
            {
                case EventState.Waiting: UpdateWaitingState(ref text); break;
                case EventState.Playing: UpdatePlayingState(ref text); break;
                case EventState.Stopping: UpdateStoppingState(ref text); break;
                case EventState.Ending: UpdateEndingState(ref text); break;
            }

            Extensions.Broadcast(Translation.Cycle.
                Replace("{name}", Name).
                Replace("{state}", text).
                Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsAlive)}"), 1);
        }

        /// <summary>
        /// The state in which we set the initial values for the new game
        /// </summary>
        /// <param name="text"></param>
        protected void UpdateWaitingState(ref string text)
        {
            text = Translation.RunDontTouch;

            if (_countdown.TotalSeconds > 0)
                return;

            _playerDict = new();
            foreach (Player player in Player.GetPlayers())
            {
                _playerDict.Add(player, new PlayerClass()
                {
                    Angle = 0,
                    Platform = null
                });
            }

            _countdown = new TimeSpan(0, 0, UnityEngine.Random.Range(2, 10));
            _eventState++;
        }

        /// <summary>
        /// Game cycle in which we check that the player runs around the center and does not touch the platforms
        /// </summary>
        /// <param name="text"></param>
        protected void UpdatePlayingState(ref string text)
        {
            text = Translation.RunDontTouch;

            foreach (Player player in Player.GetPlayers())
            {
                float curAngle = 180f + Mathf.Rad2Deg * (Mathf.Atan2(player.Position.z - MapInfo.Position.z, player.Position.x - MapInfo.Position.x));

                // The player can run in any direction. The main thing is that the angle changes and is not the same
                if (_playerDict[player].Angle == curAngle)
                {
                    Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                    player.Kill(Translation.StopRunning);
                }
                else
                {
                    _playerDict[player].Angle = curAngle;
                }

                // If the player touches the platform, it will explode
                foreach (GameObject platform in Platforms)
                {
                    if (Vector3.Distance(player.Position, platform.transform.position) < 1.5f)
                    {
                        if (_countdown.TotalSeconds > 0)
                        {
                            Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                            player.Kill(Translation.TouchAhead);
                        }
                    }
                }
            }

            if (_countdown.TotalSeconds > 0)
                return;

            foreach (var platform in Platforms)
            {
                platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.black;
            }

            Extensions.PauseAudio();
            _countdown = new TimeSpan(0, 0, 3);
            _eventState++;
        }

        /// <summary>
        /// The game stops and the players have to stand on the platforms
        /// </summary>
        /// <param name="text"></param>
        protected void UpdateStoppingState(ref string text)
        {
            text = Translation.StandFree;

            var tempDict = new List<KeyValuePair<Player, GameObject>>();
            foreach (GameObject platform in Platforms)
            {
                foreach (Player player in Player.GetPlayers())
                {
                    if (_playerDict.Any(kvPair => kvPair.Value.Platform == platform))
                        continue;

                    if (Vector3.Distance(player.Position, platform.transform.position) < 1.5f)
                    {
                        platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.red;
                        tempDict.Add(new KeyValuePair<Player, GameObject>(player, platform));
                    }
                }
            }

            foreach (var item in tempDict)
            {
                _playerDict[item.Key].Platform = item.Value;
            }
            
            if (_countdown.TotalSeconds > 0)
                return;

            _countdown = new TimeSpan(0, 0, 3);
            _eventState++;
        }

        /// <summary>
        /// Kill players who did not manage to stand on the platforms
        /// </summary>
        /// <param name="text"></param>
        protected void UpdateEndingState(ref string text)
        {
            text = Translation.StandFree;

            foreach (Player player in Player.GetPlayers().Where(r => r.IsAlive))
            {
                if (_playerDict[player].Platform is null)
                {
                    player.Kill(Translation.NoTime);
                    Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                }
            }

            if (_countdown.TotalSeconds > 0)
                return;

            Extensions.ResumeAudio();
            _countdown = new TimeSpan(0, 0, 3);
            _eventState = 0;

            foreach (var platform in Platforms)
            {
                platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.yellow;
            }
        }

        protected override void OnFinished()
        {
            string text = string.Empty;
            int count = Player.GetPlayers().Count(r => r.IsAlive);

            if (count > 1)
            {
                text = Translation.MorePlayers.Replace("{name}", Name);
            }
            else if (count == 1)
            {
                Player winner = Player.GetPlayers().Where(r => r.IsAlive).FirstOrDefault();
                text = Translation.Winner.Replace("{name}", Name).Replace("{winner}", winner.Nickname);
            }
            else
            {
                text = Translation.AllDied.Replace("{name}", Name);
            }

            Extensions.Broadcast(text, 10);
        }

        protected override void OnCleanup()
        {
            foreach (GameObject platform in Platforms)
            {
                GameObject.Destroy(platform);
            }
        }
    }
}