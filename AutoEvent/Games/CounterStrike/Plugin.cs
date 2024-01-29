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
using PlayerRoles;
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
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        [EventConfig]
        public Config Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "de_dust2", 
            Position = new Vector3(0, 30, 30),
            IsStatic = true,
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Survival.ogg", 
            Volume = 10,
            Loop = false
        };

        private EventHandler _eventHandler;
        private List<GameObject> _walls;
        internal BombState BombState;
        internal Player Winner;
        internal GameObject BombObject;
        internal TimeSpan RoundTime;
        internal List<GameObject> BombPoints;
        internal List<string> KillInfo;
        internal List<GameObject> Buttons;

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);

            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PickUpItem += _eventHandler.OnPickUpItem;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PickUpItem -= _eventHandler.OnPickUpItem;

            _eventHandler = null;
        }

        protected override void OnStart()
        {
            Winner = null;
            KillInfo = new List<string>();
            _walls = new List<GameObject>();
            BombObject = new GameObject();
            Buttons = new List<GameObject>();
            BombState = BombState.NoPlanted;
            RoundTime = new TimeSpan(0, 0, Config.TotalTimeInSeconds);
            List<GameObject> ctSpawn = new List<GameObject>();
            List<GameObject> tSpawn = new List<GameObject>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Spawnpoint_Counter": ctSpawn.Add(gameObject); break;
                    case "Spawnpoint_Terrorist": tSpawn.Add(gameObject); break;
                    case "Bomb": BombObject = gameObject; break;
                    case "Wall": _walls.Add(gameObject); break;
                    case "Spawnpoint_Bomb": Buttons.Add(gameObject); break;
                }
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.GiveLoadout(Config.NTFLoadouts);
                    player.Position = ctSpawn.RandomItem().transform.position
                        + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                }
                else
                {
                    player.GiveLoadout(Config.ChaosLoadouts);
                    player.Position = tSpawn.RandomItem().transform.position
                        + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                }
                count++;
            }
        }
        
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 20; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>Get ready: {time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            // We are removing the walls so that the players can walk.
            MapInfo.Map.AttachedBlocks.Where(r => r.name == "Wall").ToList()
                .ForEach(r => GameObject.Destroy(r));
        }

        protected override bool IsRoundDone()
        {
            var ctCount = Player.GetPlayers().Count(r => r.IsNTF);
            var tCount = Player.GetPlayers().Count(r => r.IsChaos);

            return !((tCount > 0 || BombState == BombState.Planted) && 
                ctCount > 0 && 
                RoundTime.TotalSeconds != 0);
        }

        protected override void ProcessFrame()
        {
            var ctCount = Player.GetPlayers().Count(r => r.IsNTF);
            var tCount = Player.GetPlayers().Count(r => r.IsChaos);
            var time = $"{RoundTime.Minutes:00}:{RoundTime.Seconds:00}";

            // Counts the time until the end of the round and changes according to the actions of the players
            TimeCounter();

            // Killboard that shows all the latest player kills.
            // string killboard = GetKillboardString();

            // Shows all players their missions
            string ctTask = string.Empty;
            string tTask = string.Empty;
            if (BombState == BombState.NoPlanted)
            {
                ctTask = Translation.StrikeNoPlantedCounter;
                tTask = Translation.StrikeNoPlantedTerror;
            }
            else if (BombState == BombState.Planted)
            {
                ctTask = Translation.StrikePlantedCounter;
                tTask = Translation.StrikePlantedTerror;
            }

            // Output of missions to broadcast and killboard to hints
            foreach (Player player in Player.GetPlayers())
            {
                string text = Translation.StrikeCycle.
                    Replace("{name}", Name).
                    Replace("{task}", player.Role == RoleTypeId.NtfSpecialist ? ctTask : tTask).
                    Replace("{ctCount}", ctCount.ToString()).
                    Replace("{tCount}", tCount.ToString()).
                    Replace("{time}", time);

                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);
            }
        }

        protected string GetKillboardString()
        {
            string killboard = $"Killboard:\n";
            for (int i = 0; i <= 3; i++)
            {
                if (i < KillInfo.Count)
                {
                    int length = Math.Min(KillInfo.ElementAt(i).Length, 10);
                    killboard += $"{KillInfo.ElementAt(i).Substring(0, length)}";
                }
            }

            return killboard;
        }
        
        protected void TimeCounter()
        {
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
                    if (player.IsAlive) 
                        player.Kill();
                }

                text = Translation.StrikePlantedWin;
                Extensions.PlayAudio("TBombWin.ogg", 15, false);
            }
            else if (BombState == BombState.Defused)
            {
                text = Translation.StrikeDefusedWin;
                Extensions.PlayAudio("CTWin.ogg", 10, false);
            }
            else if (tCount == 0)
            {
                text = Translation.StrikeCounterWin;
                Extensions.PlayAudio("CTWin.ogg", 10, false);
            }
            else if (ctCount == 0)
            {
                text = Translation.StrikeTerroristWin;
                Extensions.PlayAudio("TWin.ogg", 15, false);
            }
            else if (ctCount == 0 && tCount == 0)
            {
                text = Translation.StrikeDraw;
            }
            else
            {
                text = Translation.StrikeTimeEnded;
            }

            Extensions.Broadcast(text, 10);
        }
    }
}