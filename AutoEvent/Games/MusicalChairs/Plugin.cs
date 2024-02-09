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
        public override Version Version { get; set; } = new Version(1, 0, 3);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "MusicalChairs",
            Position = new Vector3(0, 0, 30),
            IsStatic = false
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "MusicalChairs.ogg",
            Volume = 10,
            Loop = false
        };
        private EventHandler _eventHandler;
        private State _eventState;
        private GameObject _parentPlatform;
        public List<GameObject> Platforms;
        private Dictionary<Player, PlayerClass> _playerDict;
        private TimeSpan _stageTime;
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
            _eventState = State.Starting;
            _stageTime = new TimeSpan();
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
            Platforms = Functions.GeneratePlatforms(
                count,
                _parentPlatform, 
                MapInfo.Position);

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

            _stageTime = new TimeSpan(0, 0, 5);
        }

        protected override bool IsRoundDone()
        {
            if (_stageTime.TotalSeconds > 0)
                _stageTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);

            return !(Player.GetPlayers().Count(r => r.IsAlive) > 1);
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            string text = Translation.Cycle.
                Replace("{name}", Name).
                Replace("{count}", Player.GetPlayers().Count(r => r.IsAlive).ToString());

            if (_eventState == State.Starting)
            {
                text = text.Replace("{state}", Translation.RunDontTouch);

                foreach (var platform in Platforms)
                {
                    platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.yellow;
                }

                if (_stageTime.TotalSeconds <= 0)
                {
                    _playerDict = new();
                    foreach (Player player in Player.GetPlayers())
                    {
                        _playerDict.Add(player, new PlayerClass()
                        {
                            Angle = 0,
                            Platform = null
                        });
                    }
                    _stageTime = new TimeSpan(0, 0, UnityEngine.Random.Range(2, 10));
                    _eventState = State.Playing;
                }
            }
            else if (_eventState == State.Playing)
            {
                text = text.Replace("{state}", Translation.RunDontTouch);

                foreach (Player player in Player.GetPlayers())
                {
                    // check the difference between the current angle and the remembered one
                    float curAngle = 180f + Mathf.Rad2Deg * (Mathf.Atan2(player.Position.z - MapInfo.Position.z, player.Position.x - MapInfo.Position.x));

                    if (_playerDict[player].Angle == curAngle)
                    {
                        Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                        player.Kill(Translation.StopRunning);
                    }
                    else
                    {
                        _playerDict[player].Angle = curAngle;
                    }

                    // Player touched platform
                    foreach(GameObject platform in Platforms)
                    {
                        if (Vector3.Distance(player.Position, platform.transform.position) < 1.5f)
                        {
                            if (_stageTime.TotalSeconds > 0)
                            {
                                Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                                player.Kill(Translation.TouchAhead);
                            }
                        }
                    }
                }

                if (_stageTime.TotalSeconds <= 0)
                {
                    foreach (var platform in Platforms)
                    {
                        platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.black;
                    }

                    Extensions.PauseAudio();
                    _stageTime = new TimeSpan(0, 0, 3);
                    _eventState = State.Stopping;
                }
            }
            else if (_eventState == State.Stopping)
            {
                text = text.Replace("{state}", Translation.StandFree);

                foreach (GameObject platform in Platforms)
                {
                    foreach (Player player in Player.GetPlayers())
                    {
                        // If the player's platform is included in the list, then skip
                        if (_playerDict.Any(kvPair => kvPair.Value.Platform == platform))
                            continue;

                        if (Vector3.Distance(player.Position, platform.transform.position) < 1.5f)
                        {
                            platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.red;
                            _playerDict[player].Platform = platform;
                        }
                    }
                }
                
                if (_stageTime.TotalSeconds <= 0)
                {
                    _stageTime = new TimeSpan(0, 0, 3);
                    _eventState = State.Ending;
                }
            }
            else if (_eventState == State.Ending)
            {
                text = text.Replace("{state}", Translation.StandFree);

                foreach (Player player in Player.GetPlayers())
                {
                    if (_playerDict[player].Platform is null)
                    {
                        Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                        player.Kill(Translation.NoTime);
                    }
                }
                
                if (_stageTime.TotalSeconds <= 0)
                {
                    Extensions.ResumeAudio();
                    _stageTime = new TimeSpan(0, 0, 3);
                    _eventState = State.Starting;
                }
            }

            Extensions.Broadcast(text, 1);
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

            foreach (GameObject platform in Platforms)
            {
                GameObject.Destroy(platform);
            }
        }
    }
}