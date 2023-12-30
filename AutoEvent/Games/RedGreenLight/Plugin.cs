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

namespace AutoEvent.Games.Light
{
    public class Plugin : Event, IEventMap, IInternalEvent, IEventTag
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.LightTranslation.LightName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.LightTranslation.LightDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.LightTranslation.LightCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        private LightTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.LightTranslation;
        [EventConfig]
        public Config Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "RedLightGreenLight",
            Position = new Vector3(0, 0, 30)
        };
        public TagInfo TagInfo { get; set; } = new TagInfo()
        {
            Name = "Christmas",
            Color = "#42aaff"
        };
        private EventHandler EventHandler { get; set; }
        GameObject Wall { get; set; }
        GameObject Doll { get; set; }
        GameObject RedLine { get; set; }
        TimeSpan ActiveTime { get; set; }
        State EventState { get; set; }
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
            RedLine = null;
            EventState = State.GreenLight;
            List<GameObject> spawnpoints = new List<GameObject>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Spawnpoint": spawnpoints.Add(gameObject); break;
                    case "Wall": Wall = gameObject; break;
                    case "RedLine": RedLine = gameObject; break;
                    case "Doll": Doll = gameObject; break;
                }
            }

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
                string text = Translation.LightStart.Replace("{time}", time.ToString());
                Extensions.Broadcast(text, 1);
                yield return Timing.WaitForSeconds(1f);
            }

            yield return Timing.WaitForSeconds(0f);
        }

        protected override void CountdownFinished()
        {
            GameObject.Destroy(Wall);
        }

        protected override bool IsRoundDone()
        {
            if (ActiveTime.TotalSeconds > 0)
            {
                ActiveTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
            }
            else ActiveTime = TimeSpan.Zero;

            return !(EventTime.TotalSeconds < Config.TotalTimeInSeconds && 
                Player.GetPlayers().Count(r => r.IsAlive) > 0);
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            if (EventState == State.GreenLight)
            {
                if (ActiveTime == TimeSpan.Zero)
                {
                    Doll.transform.rotation = Quaternion.identity;
                    int seconds = UnityEngine.Random.Range(1, 4);
                    ActiveTime = TimeSpan.FromSeconds(seconds);
                }
                else
                {
                    Extensions.PlayAudio("RedLight.ogg", 10, false);
                    EventState = State.RotatingEnable;
                }
            }
            else if (EventState == State.RotatingEnable)
            {
                Vector3 rotation = Doll.transform.rotation.eulerAngles;

                if (Mathf.Abs(rotation.y - 180) > 0.01f)
                {
                    rotation.y += 20;
                    Doll.transform.rotation = Quaternion.Euler(rotation);
                }
                else
                {
                    EventState = State.RedLight;
                }
            }
            else if (EventState == State.RedLight)
            {
                foreach(Player player in Player.GetPlayers())
                {
                    if ((int)RedLine.transform.position.z <= (int)player.Position.z)
                    {
                        continue;
                    }

                    if (player.Velocity != Vector3.zero)
                    {
                        player.Kill();
                        Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                        //Extensions.PlayAudio("Siren.ogg", 10, false);
                        ActiveTime += TimeSpan.FromSeconds(1f);
                    }
                }

                if (ActiveTime == TimeSpan.Zero)
                {
                    Extensions.PlayAudio("GreenLight.ogg", 10, false);
                    EventState = State.RotatingDisable;
                }
            }
            else if (EventState == State.RotatingDisable)
            {
                Vector3 rotation = Doll.transform.rotation.eulerAngles;

                if (Mathf.Abs(rotation.y - 0) > 0.01f)
                {
                    rotation.y -= 20;
                    Doll.transform.rotation = Quaternion.Euler(rotation);
                }
                else
                {
                    EventState = State.GreenLight;
                }
            }

            Extensions.Broadcast($"{EventState}", 1);
        }

        protected override void OnFinished()
        {
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            /*

            if (classDCount < 1 && sciCount < 1)
            {
                string text = Translation.SnowballAllDied.
                    Replace("{name}", Name).
                    Replace("{time}", time);
                Extensions.Broadcast(text, 10);
            }
            */
        }
    }
    public enum State
    {
        GreenLight,
        RotatingEnable,
        RedLight,
        RotatingDisable
    }
}