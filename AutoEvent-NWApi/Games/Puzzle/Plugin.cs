using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.API.Schematic.Objects;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AdminToys;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
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
        [EventConfig]
        public PuzzleConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Puzzle", Position = new Vector3(76f, 1026.5f, -43.68f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Puzzle.ogg", Volume = 15, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private PuzzleTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.PuzzleTranslate;
        private readonly string _broadcastName = "<color=#F59F00>P</color><color=#F68523>u</color><color=#F76B46>z</color><color=#F85169>z</color><color=#F9378C>l</color><color=#FA1DAF>e</color>";
        /// <summary>
        /// A local list of platforms that changes round to round.
        /// </summary>
        private List<GameObject> _listPlatforms;
        /// <summary>
        /// All platforms in the map.
        /// </summary>
        private List<GameObject> _platforms;
        private GameObject _lava;
        private int _stage;
        private readonly int _finaleStage = 10;
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
            _platforms = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Platform").ToList();
            _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
            _lava.AddComponent<LavaComponent>();

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
                Extensions.Broadcast($"{_broadcastName}\n{Translation.PuzzleStart.Replace("{time}", $"{time}")}", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            _stage = 1;
            _speed = 5;
            _timeDelay = 0.5f;
            _listPlatforms = _platforms;

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
            _stageText = Translation.PuzzleStage
                    .Replace("{stageNum}", $"{_stage}")
                    .Replace("{stageFinal}", $"{_finaleStage}")
                    .Replace("{plyCount}", $"{Player.GetPlayers().Count(r => r.IsAlive)}");
                /*for (float time = speed * 2; time > 0; time--)
                {
                    foreach (var platform in Platformes)
                    {
                        platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                    }
                    
                    Extensions.Broadcast($"<b>{Name}</b>\n{stageText}", 1);
                    yield return Timing.WaitForSeconds(timing);
                }*/
                for (float time = _speed * 2; time > 0; time--)
                {
                    foreach (var platform in _platforms)
                    {
                        platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor =
                            new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                    }

                    Extensions.Broadcast($"<b>{Name}</b>\n{_stageText}", 1);
                    yield return Timing.WaitForSeconds(_timeDelay);
                }


                /*var randPlatform = ListPlatformes.RandomItem();
                ListPlatformes = new List<GameObject>();
                randPlatform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.green;*/
                
                var randPlatform = _listPlatforms.RandomItem();
                _listPlatforms = new List<GameObject>();
                randPlatform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.green;

                /*foreach (var platform in Platformes)
                {
                    if (platform != randPlatform)
                    {
                        platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.magenta;
                        ListPlatformes.Add(platform);
                    }
                }*/
                foreach (var platform in _platforms)
                {
                    if (platform != randPlatform)
                    {
                        platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.magenta;
                        _listPlatforms.Add(platform);
                    }
                }
                /*Extensions.Broadcast($"<b>{_broadcastName}</b>\n{stageText}", (ushort)(speed + 1));
                yield return Timing.WaitForSeconds(speed);*/
                
                Extensions.Broadcast($"<b>{_broadcastName}</b>\n{_stageText}", (ushort)(_speed + 1));
                yield return Timing.WaitForSeconds(_speed);

                /*foreach (var platform in Platformes)
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.down * 5;
                    }
                }*/
                
                foreach (var platform in _platforms)
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.down * 5;
                    }
                }
                /*Extensions.Broadcast($"<b>{_broadcastName}</b>\n{stageText}", (ushort)(speed + 1));
                yield return Timing.WaitForSeconds(speed); */
                
                Extensions.Broadcast($"<b>{_broadcastName}</b>\n{_stageText}", (ushort)(_speed + 1));
                yield return Timing.WaitForSeconds(_speed);
                
                /*foreach (var platform in )
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.up * 5;
                    }
                }*/
                foreach (var platform in _platforms)
                {
                    if (platform != randPlatform)
                    {
                        platform.transform.position += Vector3.up * 5;
                    }
                }
                Extensions.Broadcast($"<b>{_broadcastName}</b>\n{_stageText}", (ushort)(_speed + 1));
                yield return Timing.WaitForSeconds(_speed);

                _speed -= 0.4f;
                _stage++;
                _timeDelay -= 0.04f;
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
    }
}
