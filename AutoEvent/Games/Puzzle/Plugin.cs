using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MEC;
using PlayerRoles;
using MER.Lite.Objects;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AdminToys;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Mirror;
using Random = UnityEngine.Random;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Puzzle
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    { //todo: add some configs for a platform choose speed, and platform fall delay. Maybe make this a scale from 1 - 10 or something.
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.PuzzleTranslate.PuzzleName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.PuzzleTranslate.PuzzleDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.PuzzleTranslate.PuzzleCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }

        [EventConfigPreset] public Config ColorMatch => Preset.ColorMatch;
        [EventConfigPreset] public Config Run => Preset.Run;
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "Puzzle", Position = new Vector3(76f, 1026.5f, -43.68f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "ChristmasMusic.ogg", Volume = 7, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private GridSelector GridSelector { get; set; }
        private PuzzleTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.PuzzleTranslate;
        private readonly string _broadcastName = "<color=#F59F00>P</color><color=#F68523>u</color><color=#F76B46>z</color><color=#F85169>z</color><color=#F9378C>l</color><color=#FA1DAF>e</color>";
        /// <summary>
        /// A local list of platforms that changes round to round.
        /// </summary>
        private List<GameObject> _listPlatforms;

        private List<GameObject> _colorIndicators;

        /// <summary>
        /// All platforms in the map.
        /// </summary>
        private Dictionary<ushort, GameObject> _platforms; 
        private GameObject _lava;
        private int _stage;
        private int _finaleStage => Config.Rounds;
        private float _speed = 5;
        private float _timeDelay = 0.5f;
        private string _stageText;

        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropItem -= EventHandler.OnDropItem;
            Players.DropAmmo -= EventHandler.OnDropAmmo;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            GridSelector = new GridSelector(Config.PlatformsOnEachAxis, Config.PlatformsOnEachAxis, Config.Salt, Config.SeedMethod);
            // _platforms = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Platform").ToList();
            _platforms = new Dictionary<ushort, GameObject>();
            _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
            _lava.AddComponent<LavaComponent>();
            _colorIndicators = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Cube").ToList();
            GeneratePlatforms(Config.PlatformsOnEachAxis);
            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
            }
        }

        private void GeneratePlatforms(int amountPerAxis = 5)
        {
            
            float areaSizeX = 20f;
            float areaSizeY = 20f;
            float sizeX = areaSizeX / amountPerAxis;
            float sizeY = areaSizeY / amountPerAxis;
            float startPosX = -(areaSizeX/2f) + sizeX / 2f;
            float startPosY = -(areaSizeY/2f) + sizeY / 2f;
            float breakSize = .2f;
            List<Platform> platforms = new List<Platform>();
            for (int x = 0; x < amountPerAxis; x++)
            {
                for (int y = 0; y < amountPerAxis; y++)
                {
                    float posX = startPosX + (sizeX * x);
                    float posY = startPosY + (sizeY * y);
                    var plat = new Platform(sizeX - breakSize, sizeY - breakSize, posX, posY);
                    platforms.Add(plat);
                }
            }
            var primary = MapInfo.Map.AttachedBlocks.FirstOrDefault(x => x.name == "Platform");
            foreach(var plat in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Platform"))
            {
                if (plat.GetInstanceID() != primary.GetInstanceID())
                    GameObject.Destroy(plat);
            }

            ushort id = 0;
            foreach (Platform platform in platforms)
            {
                Vector3 position = MapInfo.Map.Position + new Vector3(platform.PositionX, 5.42f ,platform.PositionY);
                var newPlatform = GameObject.Instantiate(primary, position, Quaternion.identity);
                _platforms.Add(id, newPlatform);
                var prim = newPlatform.GetComponent<PrimitiveObjectToy>() ?? newPlatform.AddComponent<PrimitiveObjectToy>();
                
                NetworkServer.UnSpawn(newPlatform);
                prim.Scale = new Vector3(platform.X , 5f, platform.Y);
                prim.NetworkScale = new Vector3(platform.X , 5f, platform.Y);
                prim.PrimitiveType = PrimitiveType.Cube;
                prim.transform.localScale = new Vector3(platform.X, 5f, platform.Y);
                NetworkServer.Spawn(newPlatform);
                id++;
            }

            GameObject.Destroy(primary);
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"{_broadcastName}\n{Translation.PuzzleStart.Replace("{time}", $"{time}")}", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            _stage = 1;
            _speed = 5;
            _timeDelay = 0.5f;
            _listPlatforms = _platforms.Values.ToList();
        }

        protected override bool IsRoundDone()
        {
            // Stage is smaller than the final stage &&
            // at least one player is alive.
            return !(_stage <= _finaleStage && Player.GetPlayers().Count(r => r.IsAlive) > 0);
        }

        protected override IEnumerator<float> RunGameCoroutine()
        {
            while (!IsRoundDone() || DebugLogger.AntiEnd)
            {
                if (KillLoop)
                {
                    yield break;
                }
                var puzzleCoroutine = Timing.RunCoroutine(PuzzleCoroutine(), "Puzzle Coroutine");
                yield return Timing.WaitUntilDone(puzzleCoroutine);

                EventTime = EventTime.Add(new TimeSpan(0, 0,1));
                yield return Timing.WaitForSeconds(1f);
            }
            yield break;
        }

        public IEnumerator<float> PuzzleCoroutine()
        {
            float selectionDelay = Config.SelectionTime.GetValue(_stage, 10,0,10);
            float fallDelay = Config.FallDelay.GetValue(_stage, 10, .3f,8);
            _stageText = Translation.PuzzleStage
                .Replace("{stageNum}", $"{_stage}")
                .Replace("{stageFinal}", $"{_finaleStage}")
                .Replace("{plyCount}", $"{Player.GetPlayers().Count(r => r.IsAlive)}");

            // Wait for platform selection.
            int spread = (int)Config.PlatformSpread.GetValue(_stage, Config.Rounds, 1, 3);
            float hueOffset = Config.HueDifficulty.GetValue(_stage, Config.Rounds, 0, 1);
            float satOffset = Config.SaturationDifficulty.GetValue(_stage, Config.Rounds, 0, 1);
            float vOffset = Config.VDifficulty.GetValue(_stage, Config.Rounds, 0, 1);
            int safePlatformCount = (int)Config.NonFallingPlatforms.GetValue(_stage, Config.Rounds, 1, 100);
            var data = GridSelector.SelectGridItem((byte)spread, true, null, hueOffset, satOffset, vOffset, safePlatformCount);
            DebugLogger.LogDebug($"Stage {_stage}: spread: {spread}, platformCount: {safePlatformCount}, hsv: {hueOffset}, {satOffset}, {vOffset}");
            var color = new Color(data.SafePoints.First().Value.R / 255f, data.SafePoints.First().Value.G / 255f,
                data.SafePoints.First().Value.B / 255f);

            if (Config.UseRandomPlatformColors)
            {
                foreach (GameObject colorIndicator in _colorIndicators)
                {
                    colorIndicator.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = color;
                }
            }
            for (float time = (float)Math.Ceiling(selectionDelay / _timeDelay); time > 0; time--)
            {
                foreach (var platform in _platforms.Values)
                {
                    platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor =
                        new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                }

                Extensions.Broadcast($"<b>{Name}</b>\n{_stageText}", 1);
                yield return Timing.WaitForSeconds(_timeDelay);
            }

            List<GameObject> nonFallingPlatforms = new List<GameObject>();
            _listPlatforms = new List<GameObject>();
            foreach (var pnt in data.SafePoints)
            {
                nonFallingPlatforms.Add(_platforms[pnt.Key]);
            }
            try
            {
                foreach (var plat in nonFallingPlatforms)
                {

                    if (Config.UseRandomPlatformColors)
                    {
                        plat.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = color;
                    }
                    else
                        plat.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.green;
                }
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"Caught an exception while selecting colors. Exception: \n{e}");
            }
            // Platform Selection.

            // Platforms have been selected.
            foreach (var kvp in _platforms)
            {
                if (!nonFallingPlatforms.Contains(kvp.Value))
                {
                    if (Config.UseRandomPlatformColors)
                    {
                        try
                        {
                            kvp.Value.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = new Color(
                                data.Points[kvp.Key].R / 255f, data.Points[kvp.Key].G / 255f,
                                data.Points[kvp.Key].B / 255f);
                        }
                        catch (Exception e)
                        {
                            DebugLogger.LogDebug("Caught an exception while processing custom colors.", LogLevel.Warn, true);
                            DebugLogger.LogDebug($"{e}");
                            kvp.Value.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.magenta;
                            
                        }
                    }
                    else
                        kvp.Value.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.magenta;
                    _listPlatforms.Add(kvp.Value);
                }
            }
                
            
            // Delay before fall.
            Extensions.Broadcast($"<b>{_broadcastName}</b>\n{_stageText}", (ushort)(fallDelay + 1));
            yield return Timing.WaitForSeconds(fallDelay);

            // Platforms Fall.
            foreach (var platform in _platforms.Values)
            {
                if (!nonFallingPlatforms.Contains(platform))
                {
                    platform.transform.position += Vector3.down * 5;
                }
            }
               
            // Wait for platforms to return.
            Extensions.Broadcast($"<b>{_broadcastName}</b>\n{_stageText}", (ushort)(fallDelay + 1));
            yield return Timing.WaitForSeconds(fallDelay);
                
            // Platforms Return.
            foreach (var platform in _platforms.Values)
            {
                if (!nonFallingPlatforms.Contains(platform))
                {
                    platform.transform.position += Vector3.up * 5;
                }
            }
            Extensions.Broadcast($"<b>{_broadcastName}</b>\n{_stageText}", (ushort)(_speed + 1.5f));
            yield return Timing.WaitForSeconds(_speed);

            _speed -= 0.39f;
            _stage++;
            _timeDelay -= 0.039f;
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.IsAlive) < 1)
            {
                Extensions.Broadcast($"<b>{_broadcastName}</b>\n{Translation.PuzzleAllDied}", 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                var player = Player.GetPlayers().First(r => r.IsAlive).DisplayNickname;
                Extensions.Broadcast($"<b>{_broadcastName}</b>\n{Translation.PuzzleWinner.Replace("{winner}", $"{player}")}", 10);
            }
            else
            {
                Extensions.Broadcast($"<b>{_broadcastName}</b>\n{Translation.PuzzleSeveralSurvivors}", 10);
            }
        }

        protected override void OnCleanup()
        {
            foreach (var platform in this._platforms)
            {
                GameObject.Destroy(platform.Value);
            }
            base.OnCleanup();
        }
    }
}

public class Platform
{
    public Platform(float sizeX, float sizeY, float positionX, float positionY){
        X = sizeX;
        Y = sizeY;
        PositionX = positionX;
        PositionY = positionY;
    }

    public GameObject GameObject { get; set; }
    public ushort PlatformId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}