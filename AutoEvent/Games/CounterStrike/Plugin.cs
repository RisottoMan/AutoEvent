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
using Event = AutoEvent.Interfaces.Event;
using CustomPlayerEffects;

namespace AutoEvent.Games.CounterStrike
{
    public class Plugin : Event, IEventMap, IInternalEvent
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
            Position = new Vector3(0, 0, 0)
        };
        public BombState BombState { get; set; }
        public double TotalTime { get; set; } = 105;
        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler(this);

            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }
        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropAmmo -= EventHandler.OnDropAmmo;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            BombState = BombState.NoPlanted;

            List<GameObject> spawnpoints = RandomClass.GetAllSpawnpoints(MapInfo.Map);
            List<GameObject> ctSpawn = spawnpoints.Where(r => r.name == "Spawnpoint_Counter").ToList();
            List<GameObject> tSpawn = spawnpoints.Where(r => r.name == "Spawnpoint_Terrorist").ToList();

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.GiveLoadout(Config.NTFLoadouts);
                    player.Position = ctSpawn.RandomItem().transform.position;
                        //+ new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                }
                else
                {
                    player.GiveLoadout(Config.ChaosLoadouts);
                    player.Position = tSpawn.RandomItem().transform.position;
                        //+ new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                }

                player.EffectsManager.EnableEffect<Ensnared>();
                count++;
            }
        }
        
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
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
            return !(TotalTime > EventTime.TotalSeconds && ctCount > 0 && tCount > 0);
        }

        protected override void ProcessFrame()
        {
            var ctCount = Player.GetPlayers().Count(r => r.IsNTF);
            var tCount = Player.GetPlayers().Count(r => r.IsChaos);
            double remainTime = TotalTime - EventTime.TotalSeconds;
            var time = $"{(int)(remainTime/60):00}:{(int)(remainTime%60):00}";

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

                /*
                if (BombState == BombState.Planted)
                {
                    EventTime = new TimeSpan(0, 0, 35);
                    Extensions.PlayAudio("BombPlanted", 5, false, "Bomb Planted");
                }
                */

                string text = Translation.StrikeCycle.
                    Replace("{name}", Name).
                    Replace("{task}", task).
                    Replace("{ctCount}", ctCount.ToString()).
                    Replace("{tCount}", tCount.ToString()).
                    Replace("{time}", time);

                player.ClearBroadcasts();
                player.SendBroadcast(Translation.StrikeCycle, 1);
            }
        }

        protected override void OnFinished()
        {
            var ctCount = Player.GetPlayers().Count(r => r.IsNTF);
            var tCount = Player.GetPlayers().Count(r => r.IsChaos);

            string text = string.Empty;
            if (ctCount == 0 && tCount == 0)
            {
                text = Translation.StrikeDraw;
            }
            else if (tCount == 0)
            {
                text = Translation.StrikeCounterWin;
            }
            else if (ctCount == 0)
            {
                text = Translation.StrikeTerroristWin;
            }
            else if (BombState == BombState.Exploded)
            {
                text = Translation.StrikePlantedWin;
            }
            else if (BombState == BombState.Defused)
            {
                text = Translation.StrikeDefusedWin;
            }

            Extensions.Broadcast(text, 10);
        }
    }
}