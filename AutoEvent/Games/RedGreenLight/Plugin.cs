using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace AutoEvent.Games.Light
{
    public class Plugin : Event<Config, Translation>, IEventMap
    {
        public override string Name { get; set; } = "Red Light Green Light";
        public override string Description { get; set; } = "Reach the end of the finish line";
        public override string Author { get; set; } = "RisottoMan";
        public override string CommandName { get; set; } = "light";
        public MapInfo MapInfo { get; set; } = new()
        {
            MapName = "RedLight",
            Position = new Vector3(0f, 40f, 0f),
            IsStatic = false
        };

        private EventHandler _eventHandler;
        private GameObject _wall;
        private GameObject _doll;
        private GameObject _redLine;
        private Animator _animator;
        private float _countdown;
        private EventState _eventState;
        internal Dictionary<Player, float> PushCooldown;
        private Dictionary<Player, Quaternion> _playerRotation;

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            Exiled.Events.Handlers.Player.Hurt += _eventHandler.OnHurt;
            Exiled.Events.Handlers.Player.TogglingNoClip += _eventHandler.OnTogglingNoclip;
        }

        protected override void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurt -= _eventHandler.OnHurt;
            Exiled.Events.Handlers.Player.TogglingNoClip -= _eventHandler.OnTogglingNoclip;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _redLine = null;
            _doll = null;
            _eventState = 0;
            PushCooldown = new Dictionary<Player, float>();
            List<GameObject> spawnpoints = new List<GameObject>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Spawnpoint": spawnpoints.Add(gameObject); break;
                    case "Wall": _wall = gameObject; break;
                    case "RedLine": _redLine = gameObject; break;
                    case "Doll":
                    {
                        _doll = gameObject;
                        _animator = _doll.GetComponent<Animator>();
                        break;
                    }
                }
            }

            foreach (Player player in Player.List)
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
            Object.Destroy(_wall);
        }

        protected override bool IsRoundDone()
        {
            int aliveCount = Player.List.Count(r => r.IsAlive);
            int lineCount = Player.List.Count(player => player.Position.z > _redLine.transform.position.z);
            if (aliveCount == lineCount)
            {
                return true;
            }

            _countdown = _countdown > 0 ? _countdown - FrameDelayInSeconds : 0;
            return !(EventTime.TotalSeconds < Config.TotalTimeInSeconds && aliveCount > 0);
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

            foreach (Player player in Player.List)
            {
                if (Config.IsEnablePush)
                    player.ShowHint(Translation.Hint, 1);

                player.ClearBroadcasts();
                player.Broadcast(1, Translation.Cycle.
                    Replace("{name}", Name).
                    Replace("{state}", text).
                    Replace("{time}", $"{Config.TotalTimeInSeconds - (int)EventTime.TotalSeconds}"));
            }
        }

        protected void UpdateGreenLightState(ref string text)
        {
            text = Translation.GreenLight;

            if (_countdown > 0)
                return;

            Extensions.PlayAudio("RedLight.ogg", 10, false);
            _animator?.Play("RedLightAnimation");
            _countdown = Random.Range(4, 8);
            _eventState++;
        }

        protected void UpdateRotateState(ref string text)
        {
            text = Translation.RedLight;
            
            if (_animator != null && !_animator.GetCurrentAnimatorStateInfo(0).IsName("PauseAnimation"))
                return;
            
            _playerRotation = new Dictionary<Player, Quaternion>();
            foreach (Player player in Player.List)
            {
                _playerRotation.Add(player, player.CameraTransform.rotation);
            }

            _eventState++;
        }

        protected void UpdateRedLightState(ref string text)
        {
            text = Translation.RedLight;

            foreach (Player player in Player.List)
            {
                if ((int)_redLine.transform.position.z <= (int)player.Position.z)
                    continue;

                Vector3 camera = _doll.transform.position + new Vector3(0, 10, 0);
                Vector3 distance = player.Position - camera;
                Physics.Raycast(camera, distance.normalized, out RaycastHit raycastHit, distance.magnitude);

                if (raycastHit.collider == null || raycastHit.collider.gameObject.layer != 13)
                    continue;

                if (!_playerRotation.ContainsKey(player))
                    continue;

                if (player.Velocity == Vector3.zero &&
                    Quaternion.Angle(_playerRotation[player], player.CameraTransform.rotation) < 10)
                    continue;

                Extensions.GrenadeSpawn(player.Position, 0.1f, 0.1f, 0);
                player.Kill(Translation.RedLose);
                _countdown++;
            }

            if (_countdown > 0)
                return;

            Extensions.PlayAudio("GreenLight.ogg", 10, false);
            _animator?.Play("GreenLightAnimation");
            _countdown = Random.Range(1.5f, 4f);
            _eventState++;
        }

        protected void UpdateReturnState(ref string text)
        {
            text = Translation.GreenLight;

            _playerRotation.Clear();
            _eventState = 0;
        }

        protected override void OnFinished()
        {
            foreach (Player player in Player.List)
            {
                if ((int)_redLine.transform.position.z > (int)player.Position.z)
                {
                    Extensions.GrenadeSpawn(player.Position, 0.1f, 0.1f);
                    player.Kill(Translation.NoTime);
                }
            }

            string text = string.Empty;
            int count = Player.List.Count(r => r.IsAlive);

            if (count > 1)
            {
                text = Translation.MoreWin.Replace("{name}", Name).Replace("{count}", count.ToString());
            }
            else if (count == 1)
            {
                Player? winner = Player.List.FirstOrDefault(r => r.IsAlive);
                text = Translation.PlayerWin.Replace("{name}", Name).Replace("{winner}", winner?.Nickname ?? "Unknown");
            }
            else
            {
                text = Translation.AllDied.Replace("{name}", Name);
            }

            Extensions.Broadcast(text, 10);
        }
    }
}
