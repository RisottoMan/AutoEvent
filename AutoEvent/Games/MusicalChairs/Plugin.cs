using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;
using AdminToys;
using PlayerRoles.FirstPersonControl;

namespace AutoEvent.Games.MusicalChairs
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent, IEventTag
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.ChairsTranslation.ChairsName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.ChairsTranslation.ChairsDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.ChairsTranslation.ChairsCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        private ChairsTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.ChairsTranslation;
        [EventConfig]
        public Config Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "MusicalChairs",
            Position = new Vector3(0, 0, 30)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "MusicalChairs.ogg",
            Volume = 10,
            Loop = true
        };
        public TagInfo TagInfo { get; set; } = new TagInfo()
        {
            Name = "Christmas",
            Color = "#77dde7"
        };
        private EventHandler EventHandler { get; set; }
        public State EventState { get; set; }
        public List<GameObject> Platforms { get; set; }
        Dictionary<GameObject, Player> PlatfomByPlayer { get; set; }
        GameObject Cylinder { get; set; }
        TimeSpan StageTime { get; set; }
        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler(this);
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
            Players.PlayerDamage += EventHandler.OnDamage;
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
            Players.PlayerDamage -= EventHandler.OnDamage;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            EventState = State.Starting;
            StageTime = new TimeSpan();
            List<GameObject> spawnpoints = new List<GameObject>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Spawnpoint": spawnpoints.Add(gameObject); break;
                    case "Cylinder-Parent": Cylinder = gameObject; break;
                }
            }

            int count = Player.GetPlayers().Count > 20 ? 20 : Player.GetPlayers().Count - 1;
            Platforms = Functions.GeneratePlatforms(
                count,
                Cylinder, 
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
                string text = Translation.ChairsStart.Replace("{time}", time.ToString());
                Extensions.Broadcast(text, 1);
                yield return Timing.WaitForSeconds(1f);
            }

            StageTime = new TimeSpan(0, 0, 5);
        }

        protected override bool IsRoundDone()
        {
            if (StageTime.TotalSeconds > 0) 
                StageTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);

            return !(Player.GetPlayers().Count(r => r.IsAlive) > 1);
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            ChairsTranslation trans = Translation;
            string text = trans.ChairsCycle.
                Replace("{name}", Name).
                Replace("{count}", Player.GetPlayers().Count(r => r.IsAlive).ToString());

            if (EventState == State.Starting)
            {
                text = text.Replace("{state}", trans.ChairsRunDontTouch);

                foreach (var platform in Platforms)
                {
                    platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.yellow;
                }

                if (StageTime.TotalSeconds <= 0)
                {
                    PlatfomByPlayer = new Dictionary<GameObject, Player>();
                    StageTime = new TimeSpan(0, 0, UnityEngine.Random.Range(2, 10));
                    EventState = State.Playing;
                }
            }
            else if (EventState == State.Playing)
            {
                text = text.Replace("{state}", trans.ChairsRunDontTouch);

                foreach (GameObject platform in Platforms)
                {
                    foreach (Player player in Player.GetPlayers())
                    {
                        if (player.ReferenceHub.roleManager.CurrentRole is IFpcRole fpc)
                        {
                            if (fpc.FpcModule.CurrentMovementState != PlayerMovementState.Sprinting)
                            {
                                Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                                player.Kill(trans.ChairsStopRunning);
                            }
                        }

                        if (Vector3.Distance(player.Position, platform.transform.position) < 1.5f)
                        {
                            if (StageTime.TotalSeconds > 0)
                            {
                                Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                                player.Kill(trans.ChairsTouchAhead);
                            }
                        }
                    }
                }

                if (StageTime.TotalSeconds <= 0)
                {
                    foreach (var platform in Platforms)
                    {
                        platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.black;
                    }

                    Extensions.PauseAudio();
                    StageTime = new TimeSpan(0, 0, 3);
                    EventState = State.Stopping;
                }
            }
            else if (EventState == State.Stopping)
            {
                text = text.Replace("{state}", trans.ChairsStandFree);

                foreach (GameObject platform in Platforms)
                {
                    foreach (Player player in Player.GetPlayers())
                    {
                        if (PlatfomByPlayer.ContainsKey(platform) || PlatfomByPlayer.ContainsValue(player))
                            continue;

                        if (Vector3.Distance(player.Position, platform.transform.position) < 1.5f)
                        {
                            platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.red;
                            PlatfomByPlayer.Add(platform, player);
                        }
                    }
                }

                if (StageTime.TotalSeconds <= 0)
                {
                    StageTime = new TimeSpan(0, 0, 3);
                    EventState = State.Ending;
                }
            }
            else if (EventState == State.Ending)
            {
                text = text.Replace("{state}", trans.ChairsStandFree);

                foreach (Player player in Player.GetPlayers())
                {
                    if (!PlatfomByPlayer.ContainsValue(player))
                    {
                        Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                        player.Kill(trans.ChairsNoTime);
                    }
                }

                if (StageTime.TotalSeconds <= 0)
                {
                    Extensions.ResumeAudio();
                    StageTime = new TimeSpan(0, 0, 3);
                    EventState = State.Starting;
                }
            }

            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            int count = Player.GetPlayers().Count(r => r.IsAlive);
            if (count > 1)
            {
                string text = Translation.ChairsMorePlayers.
                    Replace("{name}", Name);
                Extensions.Broadcast(text, 10);
            }
            else if (count == 1)
            {
                Player winner = Player.GetPlayers().Where(r => r.IsAlive).FirstOrDefault();
                string text = Translation.ChairsWinner.
                    Replace("{name}", Name).
                    Replace("{winner}", winner.Nickname);
                Extensions.Broadcast(text, 10);
            }
            else
            {
                string text = Translation.ChairsAllDied.
                    Replace("{name}", Name);
                Extensions.Broadcast(text, 10);
            }

            foreach (GameObject platform in Platforms)
            {
                GameObject.Destroy(platform);
            }
        }
    }
    public enum State
    {
        Starting,
        Playing,
        Stopping,
        Ending
    }
}