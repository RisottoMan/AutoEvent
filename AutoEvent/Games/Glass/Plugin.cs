using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MEC;
using PlayerRoles;
using UnityEngine;
using AutoEvent.Games.Glass.Features;
using Mirror;
using CustomPlayerEffects;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using HarmonyLib;
using Object = UnityEngine.Object;
using Event = AutoEvent.Interfaces.Event;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Glass
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GlassTranslate.GlassName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GlassTranslate.GlassDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } =  AutoEvent.Singleton.Translation.GlassTranslate.GlassCommandName;
        [EventConfig] public GlassConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Glass", Position = new Vector3(76f, 1026.5f, -43.68f) };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "CrabGame.ogg", Volume = 15, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private GlassTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.GlassTranslate;
        private List<GameObject> _platforms;
        private GameObject _lava;
        private GameObject _finish;
        private int _matchTimeInSeconds;

        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Players.DropItem += EventHandler.OnDropItem;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Players.DropItem -= EventHandler.OnDropItem;

            EventHandler = null;
        }

        protected override void OnStart()
        {
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
                // player.EffectsManager.EnableEffect<MovementBoost>(0, false);
                // player.EffectsManager.Eff<MovementBoost>;
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
            var text = Translation.GlassStart;
            text = text.Replace("{plyAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString());
            text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _finish.transform.position) >= 10)
                {
                    player.Damage(500, Translation.GlassDied);
                }
            }
            
            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(Translation.GlassWinSurvived.Replace("{plyAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString()), 3);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.GlassWinner.Replace("{winner}", Player.GetPlayers().First(r =>r.IsAlive).Nickname), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) < 1)
            {
                Extensions.Broadcast(Translation.GlassFail, 10);
            }
        }

        protected override void OnCleanup()
        {
            _platforms.ForEach(Object.Destroy);
        }
    }

    public class PlatformSelector
    {
        public int PlatformCount { get; set; }
        internal string Seed { get; set; }
        internal List<PlatformData> PlatformData { get; set; }
        public int MinimumSideOffset { get; set; } = 0;
        public int MaximumSideOffset { get; set; } = 0;
        public int LeftSidedPlatforms { get; set; } = 0;
        public int RightSidedPlatforms { get; set; }= 0;
        private SeedMethod _seedMethod;
        public PlatformSelector(int platformCount, string salt, int minimumSideOffset, int maximumSideOffset, SeedMethod seedMethod)
        {
            PlatformCount = platformCount;
            PlatformData = new List<PlatformData>();
            MinimumSideOffset = minimumSideOffset;
            MaximumSideOffset = maximumSideOffset;
            _seedMethod = seedMethod;
            initSeed(salt);
            _selectPlatformSideCount();
            _createPlatforms();
            _logOutput();
        }

        private void initSeed(string salt)
        {
            var bytes = GetRandomBytes().AddRangeToArray(TextToBytes(salt));
            Seed = GetSeed(bytes);
        }
        private void _selectPlatformSideCount()
        {
            bool leftSidePriority = true;
            int seedInt = GetIntFromSeededString(Seed, 3, 4, 0);
            int percent = 50;
            int priority = 5;
            int remainder = 5;
            switch (_seedMethod)
            {
               case SeedMethod.UnityRandom:
                   Random.InitState(seedInt);
                   leftSidePriority = Random.Range(0, 2) == 1; 
                   percent = Random.Range((int)MinimumSideOffset, (int)MaximumSideOffset);
                   break;
                case SeedMethod.SystemRandom:
                    var random = new System.Random(seedInt);
                    leftSidePriority = random.Next(0,2) == 1; 
                    percent = random.Next((int)MinimumSideOffset, (int)MaximumSideOffset);
                    random.Next();
                    break;
            }
                    priority = (int)((float)PlatformCount * ((float)percent / 100f));
                    remainder = PlatformCount - priority;
                   LeftSidedPlatforms = leftSidePriority ? priority : remainder;
                   RightSidedPlatforms = leftSidePriority ? remainder : priority;
        }

        private void _createPlatforms()
        {
            List<PlatformData> data = new List<PlatformData>();
            for (int i = 0; i < LeftSidedPlatforms; i++)
            {
                data.Add(new PlatformData(true, GetIntFromSeededString(Seed, 4, 4, 1 + i)));
            }

            for (int i = 0; i < RightSidedPlatforms; i++)
            {
                data.Add(new PlatformData(false,  GetIntFromSeededString(Seed, 4, 4, 1 + i + LeftSidedPlatforms)));
            }

            PlatformData = data.OrderBy(x => x.Placement).ToList();
        }

        private void _logOutput()
        {
            DebugLogger.LogDebug($"Selecting {PlatformCount} Platforms. [{MinimumSideOffset}, {MaximumSideOffset}]   {LeftSidedPlatforms} | {RightSidedPlatforms}  Seed: {Seed}", LogLevel.Debug, false);
            foreach (var platform in PlatformData.OrderByDescending(x => x.Placement))
            {
                DebugLogger.LogDebug((platform.LeftSideIsDangerous ? "[X] [=]" : "[=] [X]") + $"  Priority: {platform.Placement}", LogLevel.Debug, false);
            }
        }
        
        
        public static int GetIntFromSeededString(string seed, int count, int offset, int amount)
        {
            string seedGen = "";
            for(int s = 0; s < count; s++) {
                int indexer = (amount * count) + s;
                while(indexer >= seed.Length)
                    indexer -= seed.Length -1 ;
                seedGen += seed[indexer].ToString();
            }

            return int.Parse(seedGen);
        }
        public static byte[] GetRandomBytes(){
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] rand = new byte[8];
            rng.GetBytes(rand);
            return rand;
        }

        public static byte[] TextToBytes(string inputText)
        {
            byte[] rand = Encoding.ASCII.GetBytes(inputText);
            return rand;
        }
        public static string GetSeed(byte[] seed)
        {
		
            var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(seed);// System.Text.Encoding.ASCII.GetBytes(seed));
		
            string newSeed = "";
            foreach(byte bytemap in bytes){
                newSeed += bytemap.ToString();
            }
		
            return newSeed;
        }
    }
    public struct PlatformData
    {
        public PlatformData(bool leftSideIsDangerous, int placement)
        {
            LeftSideIsDangerous = leftSideIsDangerous;
            Placement = placement;
        }
        public int Placement { get; set; }
        public bool LeftSideIsDangerous { get; set; }
        public bool RightSideIsDangerous
        {
            get => !LeftSideIsDangerous;
            set => LeftSideIsDangerous = !value;
        }
    }
}
