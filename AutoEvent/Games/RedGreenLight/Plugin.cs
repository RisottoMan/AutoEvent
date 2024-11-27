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
        public override string Author { get; set; } = "RisottoMan";
        public override string CommandName { get; set; } = "light";
        public override Version Version { get; set; } = new Version(1, 0, 4);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "RedLight",
            Position = new Vector3(0f, 1030f, -43.5f),
            IsStatic = false
        };
        private EventHandler _eventHandler;
        private GameObject _wall;
        private GameObject _doll;
        private GameObject _redLine;
        private float _countdown;
        private EventState _eventState;
        internal Dictionary<Player, float> PushCooldown;
        private Dictionary<Player, Quaternion> _playerRotation;
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
            _redLine = null;
            _eventState = 0;
            PushCooldown = new Dictionary<Player, float>();
            List<GameObject> spawnpoints = new List<GameObject>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch(gameObject.name)
                {
                    case "Spawnpoint": spawnpoints.Add(gameObject); break;
                    case "Wall": _wall = gameObject; break;
                    case "RedLine": _redLine = gameObject; break;
                    case "Doll": _doll = gameObject; break;
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
                string text = Translation.Start.Replace("{time}", time.ToString());
                Extensions.Broadcast(text, 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            _countdown = Random.Range(1.5f, 4);
            _doll.transform.rotation = Quaternion.identity;
            GameObject.Destroy(_wall);
        }

        protected override bool IsRoundDone()
        {
            _countdown = _countdown > 0 ? _countdown -= FrameDelayInSeconds : 0;
            return !(EventTime.TotalSeconds < Config.TotalTimeInSeconds && Player.GetPlayers().Count(r => r.IsAlive) > 0);
        }

        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            string text = string.Empty;
            switch (_eventState)
            {
                case EventState.GreenLight: UpdateGreenLightState(ref text); break;
                case EventState.Rotate: UpdateRotateState(ref text); break;
                case EventState.RedLight: UpdateRedLightState(ref text); break;
                case EventState.Return: UpdateReturnState(ref text); break;
            }

            foreach (var key in PushCooldown.Keys.ToList())
            {
                if (PushCooldown[key] > 0)
                    PushCooldown[key] -= FrameDelayInSeconds;
            }

            foreach(Player player in Player.GetPlayers())
            {
                if (Config.IsEnablePush)
                    player.ReceiveHint(Translation.Hint, 1);

                player.ClearBroadcasts();
                player.SendBroadcast(Translation.Cycle.
                Replace("{name}", Name).
                Replace("{state}", text).
                Replace("{time}", $"{Config.TotalTimeInSeconds - (int)EventTime.TotalSeconds}"), 1);
            }
        }

        protected void UpdateGreenLightState(ref string text)
        {
            text = Translation.GreenLight;

            if (_countdown > 0)
                return;

            Extensions.PlayAudio("RedLight.ogg", 10, false);
            _countdown = Random.Range(4, 8);
            _eventState++;
        }

        protected void UpdateRotateState(ref string text)
        {
            text = Translation.RedLight;
            Vector3 rotation = _doll.transform.rotation.eulerAngles;

            if (Mathf.Abs(rotation.y - 180) > 0.01f)
            {
                rotation.y += 20;
                _doll.transform.rotation = Quaternion.Euler(rotation);
            }
            else
            {
                _playerRotation = new Dictionary<Player, Quaternion>();
                Player.GetPlayers().ForEach(r => { _playerRotation.Add(r, r.Camera.rotation); });
                _eventState++;
            }
        }

        protected void UpdateRedLightState(ref string text)
        {
            text = Translation.RedLight;

            foreach (Player player in Player.GetPlayers())
            {
                if ((int)_redLine.transform.position.z <= (int)player.Position.z)
                    continue;

                Vector3 camera = _doll.transform.position + new Vector3(0, 10, 0);
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
                _countdown++;
            }

            if (_countdown > 0)
                return;

            Extensions.PlayAudio("GreenLight.ogg", 10, false);
            _countdown = Random.Range(1.5f, 4f);
            _eventState++;
        }

        protected void UpdateReturnState(ref string text)
        {
            text = Translation.GreenLight;
            Vector3 rotation = _doll.transform.rotation.eulerAngles;

            if (Mathf.Abs(rotation.y - 0) > 0.01f)
            {
                rotation.y -= 20;
                _doll.transform.rotation = Quaternion.Euler(rotation);
            }
            else
            {
                _playerRotation.Clear();
                _eventState = 0;
            }
        }

        protected override void OnFinished()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if ((int)_redLine.transform.position.z > (int)player.Position.z)
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
}