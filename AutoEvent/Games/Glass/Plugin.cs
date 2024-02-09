using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using UnityEngine;
using AutoEvent.Games.Glass.Features;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Object = UnityEngine.Object;
using Event = AutoEvent.Interfaces.Event;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Glass
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Dead Jump";
        public override string Description { get; set; } = "Jump on fragile platforms";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "glass";
        public override Version Version { get; set; } = new Version(1, 0, 2);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "Glass", 
            Position = new Vector3(76f, 1026.5f, -43.68f),
            IsStatic = true
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "CrabGame.ogg", 
            Volume = 15
        };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler _eventHandler { get; set; }
        private List<GameObject> _platforms;
        private GameObject _lava;
        private GameObject _finish;
        private int _matchTimeInSeconds;
        private TimeSpan _remaining;
        public Dictionary<Player, float> pushCooldown;
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.PlayerNoclip += _eventHandler.OnPlayerNoclip;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.PlayerNoclip -= _eventHandler.OnPlayerNoclip;

            _eventHandler = null;
        }

        protected override void OnStart()
        {
            pushCooldown = new Dictionary<Player, float>();
            _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
            _lava.AddComponent<LavaComponent>();

            int platformCount;
            int playerCount = Player.GetPlayers().Count(r => r.IsAlive);
            if (playerCount <= 5)
            {
                platformCount = 3;
                _matchTimeInSeconds = 30;
            }
            else if (playerCount > 5 && playerCount <= 15)
            {
                platformCount = 6;
                _matchTimeInSeconds = 60;
            }
            else if (playerCount > 15 && playerCount <= 25)
            {
                platformCount = 9;
                _matchTimeInSeconds = 90;
            }
            else if (playerCount > 25 && playerCount <= 30)
            {
                platformCount = 12;
                _matchTimeInSeconds = 120;
            }
            else
            {
                platformCount = 15;
                _matchTimeInSeconds = 150;
            }
            _remaining = TimeSpan.FromSeconds(_matchTimeInSeconds);

            var platform = MapInfo.Map.AttachedBlocks.First(x => x.name == "Platform");
            var platform1 = MapInfo.Map.AttachedBlocks.First(x => x.name == "Platform1");

            _platforms = new List<GameObject>();
            var delta = new Vector3(3.69f, 0, 0);
            PlatformSelector selector = new PlatformSelector(platformCount, Config.SeedSalt, Config.MinimumSideOffset, Config.MaximumSideOffset, Config.PlatformScrambleMethod);
            for (int i = 0; i < platformCount; i++)
            {
                PlatformData data;
                try
                {
                    data = selector.PlatformData[i];
                }
                catch (Exception e)
                {
                    data = new PlatformData(Random.Range(0, 2) == 1, -1);
                    DebugLogger.LogDebug("An error has occured while processing platform data.", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"selector count: {selector.PlatformCount}, selector length: {selector.PlatformData.Count}, specified count: {platformCount}, [i: {i}]");
                    DebugLogger.LogDebug($"{e}");
                }

                var newPlatform = Object.Instantiate(platform, platform.transform.position + delta * (i + 1), Quaternion.identity);
                NetworkServer.Spawn(newPlatform);
                _platforms.Add(newPlatform);

                var newPlatform1 = Object.Instantiate(platform1, platform1.transform.position + delta * (i + 1), Quaternion.identity);
                NetworkServer.Spawn(newPlatform1);
                _platforms.Add(newPlatform1);

                
                if (data.LeftSideIsDangerous)
                {
                    newPlatform.AddComponent<GlassComponent>().Init(Config.BrokenPlatformRegenerateDelayInSeconds);
                }
                else
                {
                    newPlatform1.AddComponent<GlassComponent>().Init(Config.BrokenPlatformRegenerateDelayInSeconds);
                }
            }

            _finish = MapInfo.Map.AttachedBlocks.First(x => x.name == "Finish");
            _finish.transform.position = (platform.transform.position + platform1.transform.position) / 2f + delta * (platformCount + 2);

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            GameObject.Destroy(MapInfo.Map.AttachedBlocks.First(x => x.name == "Wall"));
        }

        protected override bool IsRoundDone()
        {
            // Elapsed time is smaller then the match time (+ countdown) &&
            // At least one player is alive && 
            // At least one player is not on the platform.
            
            bool playerNotOnPlatform = false;
            foreach (Player ply in Player.GetPlayers().Where(ply => ply.IsAlive))
            {
                if (Vector3.Distance(_finish.transform.position, ply.Position) >= 4)
                {
                    playerNotOnPlatform = true;
                    break;
                }
            }
            return !(EventTime.TotalSeconds < _matchTimeInSeconds && Player.GetPlayers().Count(r => r.IsAlive) > 0 && playerNotOnPlatform);
        }

        protected override void ProcessFrame()
        {
            _remaining -= TimeSpan.FromSeconds(FrameDelayInSeconds);
            var text = Translation.Start;
            text = text.Replace("{plyAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString());
            text = text.Replace("{time}", $"{_remaining.Minutes:00}:{_remaining.Seconds:00}");

            foreach (var key in pushCooldown.Keys.ToList())
            {
                if (pushCooldown[key] > 0)
                    pushCooldown[key] -= FrameDelayInSeconds;
            }

            foreach(Player player in Player.GetPlayers())
            {
                player.ReceiveHint("<color=green>Press <color=yellow>[Alt]</color> to push the player</color>", 1);
                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);
            }
        }

        protected override void OnFinished()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _finish.transform.position) >= 10)
                {
                    player.Damage(500, Translation.Died);
                }
            }
            
            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(Translation.WinSurvived.Replace("{plyAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString()), 3);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.Winner.Replace("{winner}", Player.GetPlayers().First(r =>r.IsAlive).Nickname), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) < 1)
            {
                Extensions.Broadcast(Translation.Fail, 10);
            }
        }

        protected override void OnCleanup()
        {
            _platforms.ForEach(Object.Destroy);
        }
    }
}
