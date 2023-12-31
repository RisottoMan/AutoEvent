using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using PlayerRoles;
using InventorySystem.Items;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Snowball
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent, IEventTag
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.SnowballTranslation.SnowballName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.SnowballTranslation.SnowballDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.SnowballTranslation.SnowballCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        private SnowballTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.SnowballTranslation;
        [EventConfig]
        public Config Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "Snowball",
            Position = new Vector3(0, 0, 30)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Fall_Guys_Winter_Fallympics.ogg",
            Volume = 7, 
            Loop = true
        };
        public TagInfo TagInfo { get; set; } = new TagInfo()
        {
            Name = "Christmas",
            Color = "#42aaff"
        };
        private EventHandler EventHandler { get; set; }
        List<GameObject> Walls { get; set; }
        List<GameObject> SnowballItems { get; set; }
        List<GameObject> ClassDSpawn { get; set; }
        List<GameObject> ScientistSpawn { get; set; }
        GameObject RedLine { get; set; }
        TimeSpan RoundTime { get; set; }
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
            RedLine = null;
            Walls = new List<GameObject>();
            SnowballItems = new List<GameObject>();
            ClassDSpawn = new List<GameObject>();
            ScientistSpawn = new List<GameObject>();
            RoundTime = new TimeSpan(0, 0, Config.TotalTimeInSeconds);

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Spawnpoint_ClassD": ClassDSpawn.Add(gameObject); break;
                    case "Spawnpoint_Scientist": ScientistSpawn.Add(gameObject); break;
                    case "Wall": Walls.Add(gameObject); break;
                    case "Snowball_Item": SnowballItems.Add(gameObject); break;
                    case "RedLine": RedLine = gameObject; break;
                }
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.GiveLoadout(Config.ClassDLoadouts);
                    player.Position = ClassDSpawn.RandomItem().transform.position;
                }
                else
                {
                    player.GiveLoadout(Config.ScientistLoadouts);
                    player.Position = ScientistSpawn.RandomItem().transform.position;
                }
                count++;
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                string text = Translation.SnowballStart.Replace("{time}", time.ToString());
                Extensions.Broadcast(text, 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            foreach (GameObject wall in Walls)
            {
                GameObject.Destroy(wall);
            }
        }

        protected override bool IsRoundDone()
        {
            RoundTime -= TimeSpan.FromSeconds(0.1f);
            return !(RoundTime.TotalSeconds > 0 && 
                Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0 &&
                Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) > 0);
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            var time = $"{RoundTime.Minutes:00}:{RoundTime.Seconds:00}";
            string text = Translation.SnowballCycle.
                Replace("{name}", Name).
                Replace("{time}", time);

            foreach (Player player in Player.GetPlayers())
            {
                if ((int)RedLine.transform.position.z == (int)player.Position.z)
                {
                    if (player.Role == RoleTypeId.ClassD)
                    {
                        player.Position = ClassDSpawn.RandomItem().transform.position;
                    }
                    else
                    {
                        player.Position = ScientistSpawn.RandomItem().transform.position;
                    }

                    player.Damage(40, Translation.SnowballRedline);
                }

                foreach(GameObject ball in SnowballItems)
                {
                    if (Vector3.Distance(ball.transform.position, player.Position) < 1.5f)
                    {
                        ItemBase item = player.Items.FirstOrDefault(r => r.ItemTypeId == ItemType.Snowball);
                        if (item == null)
                        {
                            item = player.AddItem(Config.ItemType);
                        }

                        Timing.CallDelayed(.1f, () =>
                        {
                            if (item != null)
                            {
                                player.CurrentItem = item;
                            }
                        });
                    }
                }

                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);
            }
        }

        protected override void OnFinished()
        {
            TimeSpan totalTime = TimeSpan.FromSeconds(Config.TotalTimeInSeconds) - RoundTime;
            var time = $"{totalTime.Minutes:00}:{totalTime.Seconds:00}";

            int classDCount = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
            int sciCount = Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist);

            if (classDCount < 1 && sciCount < 1)
            {
                string text = Translation.SnowballAllDied.
                    Replace("{name}", Name).
                    Replace("{time}", time);
                Extensions.Broadcast(text, 10);
            }
            else if (classDCount < 1)
            {
                string text = Translation.SnowballScientistWin.
                    Replace("{name}", Name).
                    Replace("{time}", time);
                Extensions.Broadcast(text, 10);
            }
            else if (sciCount < 1)
            {
                string text = Translation.SnowballClassDWin.
                    Replace("{name}", Name).
                    Replace("{time}", time);
                Extensions.Broadcast(text, 10);
            }
            else if (RoundTime.TotalSeconds <= 0)
            {
                string text = Translation.SnowballDraw.
                    Replace("{name}", Name).
                    Replace("{time}", time);
                Extensions.Broadcast(text, 10);
            }
        }
    }
}