using MER.Lite.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.FallDown
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "FallDown";
        public override string Description { get; set; } = "All platforms are destroyed. It is necessary to survive";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "fall";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventConfigPreset] 
        public Config Warning => Preset.PlatformWarning;
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "FallDown", 
            Position = new Vector3(10f, 1020f, -43.68f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "ChristmasMusic.ogg", 
            Volume = 7
        };
        protected override float FrameDelayInSeconds { get; set; } = 0.9f;
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler _eventHandler { get; set; }
        private int _platformId { get; set; }
        private List<GameObject> _platforms;
        private GameObject _lava;
        private bool _noPlatformsRemainingWarning;

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
            _noPlatformsRemainingWarning = true;
            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }

            _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
            _lava.AddComponent<LavaComponent>().StartComponent(this);
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            _platformId = 0;
            _platforms = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Platform").ToList();
            GameObject.Destroy(MapInfo.Map.AttachedBlocks.First(x => x.name == "Wall"));
            if (Config.PlatformsHaveColorWarning)
            {
                foreach (var platform in _platforms)
                {
                    platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.white;
                }
            }
        }

        protected override bool IsRoundDone()
        {
            // Over 1 player is alive &&
            // over 1 platform is present. 
            return !(Player.GetPlayers().Count(r => r.IsAlive) > 1 && _platforms.Count > 1);
        }
        protected override void ProcessFrame()
        {
            _platformId++;
            FrameDelayInSeconds = Config.DelayInSeconds.GetValue(_platformId, 169, 1, 0.3f);

            var count = Player.GetPlayers().Count(r => r.IsAlive);
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            Extensions.Broadcast(Translation.Broadcast.Replace("{name}", Name).Replace("{time}", time).Replace("{count}", $"{count}"), (ushort)FrameDelayInSeconds);
            
            if (_platforms.Count < 1)
            {
                if (_noPlatformsRemainingWarning)
                {
                    DebugLogger.LogDebug("No platforms remaining.", LogLevel.Debug);
                    _noPlatformsRemainingWarning = false;
                }
                return;
            }
                
            var platform = _platforms.RandomItem();
            platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.red;
            if (Config.PlatformsHaveColorWarning)
            {
                Timing.CallDelayed(Config.WarningDelayInSeconds.GetValue(_platformId, 169, 0, 3), () =>
                {
                    _platforms.Remove(platform);
                    GameObject.Destroy(platform);
                });
            }
            else
            {
                _platforms.Remove(platform);
                GameObject.Destroy(platform);
            }

        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.Winner.Replace("{winner}", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.Died, 10);
            }
        }
    }
}
