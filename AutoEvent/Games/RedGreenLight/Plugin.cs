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
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Light
{
    public class Plugin : Event, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Red Light Green Light";
        public override string Description { get; set; } = "Reach the end of the finish line";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "light";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "RedLight",
            Position = new Vector3(0f, 1030f, -43.5f)
        };
        private EventHandler _eventHandler { get; set; }
        GameObject Wall { get; set; }
        GameObject Doll { get; set; }
        GameObject RedLine { get; set; }
        TimeSpan ActiveTime { get; set; }
        State EventState { get; set; }
        public Dictionary<Player, float> PushCooldown;
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDamage += _eventHandler.OnDamage;
            Players.PlayerNoclip += _eventHandler.OnPlayerNoclip;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PlayerDamage -= _eventHandler.OnDamage;
            Players.PlayerNoclip -= _eventHandler.OnPlayerNoclip;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            RedLine = null;
            EventState = State.GreenLight;
            PushCooldown = new Dictionary<Player, float>();
            List<GameObject> spawnpoints = new List<GameObject>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Spawnpoint": spawnpoints.Add(gameObject); break;
                    case "Wall": Wall = gameObject; break;
                    case "RedLine": RedLine = gameObject; break;
                    case "Doll": { DebugLogger.LogDebug("Doll найден"); Doll = gameObject; break; }
                }
            }

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadout);
                player.Position = spawnpoints.RandomItem().transform.position;
                player.ReceiveHint("<color=green>Press <color=yellow>[Alt]</color> to push the player</color>", Config.TotalTimeInSeconds);
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

            yield return Timing.WaitForSeconds(0f);
        }

        protected override void CountdownFinished()
        {
            ActiveTime = new TimeSpan(0, 0, 5);
            Doll.transform.rotation = Quaternion.identity;
            ActiveTime = TimeSpan.FromSeconds(Random.Range(1.5f, 4));

            GameObject.Destroy(Wall);
        }

        protected override bool IsRoundDone()
        {
            if (ActiveTime.TotalSeconds > 0)
            {
                ActiveTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
            }
            else ActiveTime = TimeSpan.Zero;

            foreach (var key in PushCooldown.Keys.ToList())
            {
                if (PushCooldown[key] > 0)
                    PushCooldown[key] -= FrameDelayInSeconds;
            }

            return !(EventTime.TotalSeconds < Config.TotalTimeInSeconds && 
                Player.GetPlayers().Count(r => r.IsAlive) > 0);
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected Dictionary<Player, Quaternion> _playerRotation;
        protected override void ProcessFrame()
        {
            int remainTime = Config.TotalTimeInSeconds - (int)EventTime.TotalSeconds;
            string text = Translation.Cycle.
                Replace("{name}", Name).
                Replace("{time}", remainTime.ToString());

            if (EventState == State.GreenLight)
            {
                text = text.Replace("{state}", Translation.GreenLight);

                if (ActiveTime.TotalSeconds <= 0)
                {
                    Extensions.PlayAudio("RedLight.ogg", 10, false);
                    ActiveTime = TimeSpan.FromSeconds(Random.Range(4, 8));
                    EventState = State.RotatingEnable;
                }
            }
            else if (EventState == State.RotatingEnable)
            {
                text = text.Replace("{state}", Translation.RedLight);
                Vector3 rotation = Doll.transform.rotation.eulerAngles;

                if (Mathf.Abs(rotation.y - 180) > 0.01f)
                {
                    rotation.y += 20;
                    Doll.transform.rotation = Quaternion.Euler(rotation);
                }
                else
                {
                    _playerRotation = new Dictionary<Player, Quaternion>();
                    Player.GetPlayers().ForEach(r => 
                    {
                        _playerRotation.Add(r, r.Camera.rotation);
                    });

                    EventState = State.RedLight;
                }
            }
            else if (EventState == State.RedLight)
            {
                text = text.Replace("{state}", Translation.RedLight);
                foreach (Player player in Player.GetPlayers())
                {
                    if ((int)RedLine.transform.position.z <= (int)player.Position.z)
                        continue;

                    Vector3 camera = Doll.transform.position + new Vector3(0, 10, 0);
                    Vector3 distance = player.Position - camera;
                    Physics.Raycast(camera, distance.normalized, out RaycastHit raycastHit, distance.magnitude);

                    if (raycastHit.collider.gameObject.layer != 13)
                        continue;

                    if (!_playerRotation.ContainsKey(player))
                        continue;

                    if (player.Velocity == Vector3.zero && 
                        Quaternion.Angle(_playerRotation[player], player.Camera.rotation) < 10)
                        continue;

                    Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                    player.Kill(Translation.RedLose);
                    ActiveTime += TimeSpan.FromSeconds(1f);
                }

                if (ActiveTime.TotalSeconds <= 0)
                {
                    Extensions.PlayAudio("GreenLight.ogg", 10, false);
                    EventState = State.RotatingDisable;
                    ActiveTime = TimeSpan.FromSeconds(Random.Range(1.5f, 4));
                }
            }
            else if (EventState == State.RotatingDisable)
            {
                text = text.Replace("{state}", Translation.GreenLight);
                Vector3 rotation = Doll.transform.rotation.eulerAngles;

                if (Mathf.Abs(rotation.y - 0) > 0.01f)
                {
                    rotation.y -= 20;
                    Doll.transform.rotation = Quaternion.Euler(rotation);
                }
                else
                {
                    _playerRotation.Clear();
                    EventState = State.GreenLight;
                }
            }
            
            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if ((int)RedLine.transform.position.z > (int)player.Position.z)
                {
                    Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                    player.Kill(Translation.NoTime);
                }
            }

            string text = string.Empty;
            int count = Player.GetPlayers().Count(r => r.IsAlive);

            if (count > 1)
            {
                text = Translation.MoreWin.Replace("{name}", Name).Replace("{count}", count.ToString());
            }
            else if (count == 1)
            {
                Player winner = Player.GetPlayers().Where(r => r.IsAlive).FirstOrDefault();
                text = Translation.PlayerWin.Replace("{name}", Name).Replace("{winner}", winner.Nickname);
            }
            else
            {
                text = Translation.AllDied.Replace("{name}", Name);
            }

            Extensions.Broadcast(text, 10);
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