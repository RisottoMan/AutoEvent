using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using CustomPlayerEffects;
using MER.Lite.Objects;
using Random = UnityEngine.Random;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.CounterStrike
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.StrikeTranslation.StrikeSName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.StrikeTranslation.StrikeDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.StrikeTranslation.StrikeCommandName;
        private StrikeTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.StrikeTranslation;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        private EventHandler EventHandler { get; set; }
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        [EventConfig]
        public Config Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "de_dust2", 
            Position = new Vector3(0, 0, 100)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Survival.ogg", 
            Volume = 10,
            Loop = false
        };
        public BombState BombState { get; set; }
        public Player Winner { get; set; }
        public SchematicObject BombSchematic { get; set; }
        public TimeSpan RoundTime { get; set; }
        public List<GameObject> BombPoints { get; set; }

        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler(this);

            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropAmmo += EventHandler.OnDropAmmo;
            Players.PickUpItem += EventHandler.OnPickUpItem;
        }
        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropAmmo -= EventHandler.OnDropAmmo;
            Players.PickUpItem -= EventHandler.OnPickUpItem;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            Winner = null;
            BombState = BombState.NoPlanted;
            RoundTime = new TimeSpan(0, 0, Config.TotalTimeInSeconds);

            List<GameObject> spawnpoints = Functions.GetAllSpawnpoints(MapInfo.Map);
            List<GameObject> ctSpawn = spawnpoints.Where(r => r.name == "Spawnpoint_Counter").ToList();
            List<GameObject> tSpawn = spawnpoints.Where(r => r.name == "Spawnpoint_Terrorist").ToList();
            BombPoints = spawnpoints.Where(r => r.name.Contains("Spawnpoint_Bomb")).ToList();

            foreach(GameObject gameObject in BombPoints)
            {
                Functions.CreatePlantByPoint(
                    gameObject.transform.position, 
                    gameObject.transform.rotation, 
                    gameObject.transform);
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.GiveLoadout(Config.NTFLoadouts);
                    player.Position = ctSpawn.RandomItem().transform.position
                        + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                }
                else
                {
                    player.GiveLoadout(Config.ChaosLoadouts);
                    player.Position = tSpawn.RandomItem().transform.position
                        + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                }

                player.EffectsManager.EnableEffect<Ensnared>();
                count++;
            }
        }
        
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 20; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            foreach(Player player in Player.GetPlayers())
            {
                player.EffectsManager.DisableEffect<Ensnared>();
            }
        }

        protected override bool IsRoundDone()
        {
            var ctCount = Player.GetPlayers().Count(r => r.IsNTF);
            var tCount = Player.GetPlayers().Count(r => r.IsChaos);
            RoundTime -= TimeSpan.FromSeconds(1);

            if (BombState == BombState.Planted)
            {
                if (RoundTime.TotalSeconds == 0)
                {
                    BombState = BombState.Exploded;
                }
            }
            else if (BombState == BombState.Defused)
            {
                RoundTime = new TimeSpan(0, 0, 0);
            }

            return !((tCount > 0 || BombState == BombState.Planted) && ctCount > 0 && RoundTime.TotalSeconds != 0);
        }

        protected override void ProcessFrame()
        {
            var ctCount = Player.GetPlayers().Count(r => r.IsNTF);
            var tCount = Player.GetPlayers().Count(r => r.IsChaos);
            var time = $"{RoundTime.Minutes:00}:{RoundTime.Seconds:00}";

            foreach (Player player in Player.GetPlayers())
            {
                string task = string.Empty;
                if (BombState == BombState.NoPlanted)
                {
                    if (player.IsNTF)
                    {
                        task = Translation.StrikeNoPlantedCounter;
                    }

                    if (player.IsChaos)
                    {
                        task = Translation.StrikeNoPlantedTerror;
                    }
                }
                else if (BombState == BombState.Planted)
                {
                    if (player.IsNTF)
                    {
                        task = Translation.StrikePlantedCounter;
                    }

                    if (player.IsChaos)
                    {
                        task = Translation.StrikePlantedTerror;
                    }
                }

                string text = Translation.StrikeCycle.
                    Replace("{name}", Name).
                    Replace("{task}", task).
                    Replace("{ctCount}", ctCount.ToString()).
                    Replace("{tCount}", tCount.ToString()).
                    Replace("{time}", time);

                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);
            }
        }

        protected override void OnFinished()
        {
            var ctCount = Player.GetPlayers().Count(r => r.IsNTF);
            var tCount = Player.GetPlayers().Count(r => r.IsChaos);

            string text = string.Empty;
            if (BombState == BombState.Exploded)
            {
                foreach (Player player in Player.GetPlayers())
                {
                    if (player.IsAlive) player.Kill();
                }

                text = Translation.StrikePlantedWin;
                Extensions.PlayAudio("TBombWin.ogg", 15, false, "TBombWin");
            }
            else if (BombState == BombState.Defused)
            {
                text = Translation.StrikeDefusedWin;
                Extensions.PlayAudio("CTWin.ogg", 10, false, "CTWin");
            }
            else if (tCount == 0)
            {
                text = Translation.StrikeCounterWin;
                Extensions.PlayAudio("CTWin.ogg", 10, false, "CTWin");
            }
            else if (ctCount == 0)
            {
                text = Translation.StrikeTerroristWin;
                Extensions.PlayAudio("TWin.ogg", 15, false, "TWin");
            }
            else if (ctCount == 0 && tCount == 0)
            {
                text = Translation.StrikeDraw;
            }

            Extensions.Broadcast(text, 10);
        }

        protected override void OnCleanup()
        {
            if (BombSchematic != null)
            {
                BombSchematic.Destroy();
            }
        }
    }
}