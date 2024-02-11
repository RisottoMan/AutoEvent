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

namespace AutoEvent.Games.Dodgeball
{
    public class Plugin : Event, IEventMap, IInternalEvent, IEventSound, IHidden
    {
        public override string Name { get; set; } = "Dodgeball";
        public override string Description { get; set; } = "Defeat the enemy with balls.";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "dodge";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "Dodgeball",
            Position = new Vector3(0, 0, 30)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Fall_Guys_Winter_Fallympics.ogg",
            Volume = 7
        };
        private EventHandler _eventHandler;
        private List<GameObject> _walls;
        private List<GameObject> _ballItems;
        private List<GameObject> _dPoint;
        private List<GameObject> _sciPoint;
        private GameObject _redLine;
        private TimeSpan _roundTime;
        private GameObject _ballObject;
        public List<GameObject> BallObjects;
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Servers.Scp018Bounce += _eventHandler.OnScp018Bounce;
            Players.PlayerDamage += _eventHandler.OnDamage;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Servers.Scp018Bounce -= _eventHandler.OnScp018Bounce;
            Players.PlayerDamage -= _eventHandler.OnDamage;

            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _redLine = null;
            _walls = new List<GameObject>();
            _ballItems = new List<GameObject>();
            _dPoint = new List<GameObject>();
            _sciPoint = new List<GameObject>();
            BallObjects = new List<GameObject>();
            _roundTime = new TimeSpan(0, 0, Config.TotalTimeInSeconds);

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Spawnpoint_ClassD": _dPoint.Add(gameObject); break;
                    case "Spawnpoint_Scientist": _sciPoint.Add(gameObject); break;
                    case "Wall": _walls.Add(gameObject); break;
                    case "Snowball_Item": _ballItems.Add(gameObject); break;
                    case "RedLine": _redLine = gameObject; break;
                    case "Snowball_Object": _ballObject = gameObject; break;
                }
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.GiveLoadout(Config.ClassDLoadouts);
                    player.Position = _dPoint.RandomItem().transform.position;
                }
                else
                {
                    player.GiveLoadout(Config.ScientistLoadouts);
                    player.Position = _sciPoint.RandomItem().transform.position;
                }

                count++;
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                string text = Translation.Start.Replace("{time}", time.ToString());
                Extensions.Broadcast(text, 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            foreach (GameObject wall in _walls)
            {
                GameObject.Destroy(wall);
            }
        }

        protected override bool IsRoundDone()
        {
            _roundTime -= TimeSpan.FromSeconds(0.1f);
            return !(_roundTime.TotalSeconds > 0 && 
               Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0 &&
               Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) > 0);
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            string time = $"{_roundTime.Minutes:00}:{_roundTime.Seconds:00}";
            string text = Translation.Cycle.Replace("{name}", Name).Replace("{time}", time);

            foreach (Player player in Player.GetPlayers())
            {
                foreach(GameObject ball in BallObjects)
                {
                    if (Vector3.Distance(player.Position, ball.transform.position) < 1)
                    {
                        player.Damage(50, "Damaged");
                        GameObject.Destroy(ball);
                    }
                }

                if ((int)_redLine.transform.position.z == (int)player.Position.z)
                {
                    if (player.Role == RoleTypeId.ClassD)
                    {
                        player.Position = _dPoint.RandomItem().transform.position;
                    }
                    else
                    {
                        player.Position = _sciPoint.RandomItem().transform.position;
                    }

                    player.Damage(40, Translation.Redline);
                }

                foreach(GameObject ball in _ballItems)
                {
                    if (Vector3.Distance(ball.transform.position, player.Position) < 1.5f)
                    {
                        ItemBase item = player.Items.FirstOrDefault(r => r.ItemTypeId == ItemType.SCP018);
                        if (item == null)
                        {
                            item = player.AddItem(ItemType.SCP018);
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
            TimeSpan totalTime = TimeSpan.FromSeconds(Config.TotalTimeInSeconds) - _roundTime;
            string time = $"{totalTime.Minutes:00}:{totalTime.Seconds:00}";

            int classDCount = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
            int sciCount = Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist);
            string text = string.Empty;

            if (classDCount < 1 && sciCount < 1)
            {
                text = Translation.AllDied.Replace("{name}", Name).Replace("{time}", time);
            }
            else if (classDCount < 1)
            {
                text = Translation.ScientistWin.Replace("{name}", Name).Replace("{time}", time);
            }
            else if (sciCount < 1)
            {
                text = Translation.ClassDWin.Replace("{name}", Name).Replace("{time}", time);
            }
            else if (_roundTime.TotalSeconds <= 0)
            {
                text = Translation.Draw.Replace("{name}", Name).Replace("{time}", time);
            }

            Extensions.Broadcast(text, 10);
        }
    }
}