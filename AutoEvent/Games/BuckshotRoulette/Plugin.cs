using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using MEC;
using PlayerRoles;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.BuckshotRoulette
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Buckshot Roulette";
        public override string Description { get; set; } = "One-on-one battle in Russian roulette with shotguns";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "shotgun";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "Buckshot",
            Position = new Vector3(0, 30, 30)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Knife.ogg", 
            Volume = 10, 
            Loop = true 
        };
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        private EventHandler _eventHandler;
        private Player _scientist;
        private Player _classD;
        private List<GameObject> _triggers;
        private List<GameObject> _teleports;
        private GameObject _shotgunObject;
        private TimeSpan _countdown;
        private EventState _eventState;
        private bool _isClassDMove;
        private ShotgunState _gunState;
        private Animator _animator;

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _scientist = null;
            _classD = null;
            _triggers = new();
            _teleports = new();
            _shotgunObject = new();
            _eventState = 0;
            _gunState = 0;
            _isClassDMove = true;

            if (Config.Team1Loadouts == Config.Team2Loadouts)
            {
                DebugLogger.LogDebug("Warning: Teams should not have the same roles.", LogLevel.Warn);
            }

            foreach (GameObject block in MapInfo.Map.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "Trigger": _triggers.Add(block); break;
                    case "Teleport": _teleports.Add(block); break;
                    case "Shotgun":
                    {
                        _shotgunObject = block;
                        _animator = _shotgunObject.GetComponent<Animator>();
                    }
                    break;
                }
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)     
                {              
                    player.GiveLoadout(Config.Team1Loadouts);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    player.GiveLoadout(Config.Team2Loadouts);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }
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

        protected override bool IsRoundDone()
        {
            // We decrease the counter all the time
            _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;

            return !(Player.GetPlayers().Where(r => r.Role == RoleTypeId.ClassD).Count() > 0 && 
                Player.GetPlayers().Where(r => r.Role == RoleTypeId.Scientist).Count() > 0);
        }

        protected override void ProcessFrame()
        {
            string text = string.Empty;
            switch (_eventState)
            {
                case EventState.Waiting: UpdateWaitingState(ref text); break;
                case EventState.ChooseClassD: _classD = UpdateChoosePlayerState(ref text, true); break;
                case EventState.ChooseScientist: _scientist = UpdateChoosePlayerState(ref text, false); break;
                case EventState.Playing: UpdatePlayingState(ref text); break;
                case EventState.Shooting: UpdateShootingState(ref text); break;
                case EventState.Finishing: UpdateFinishingState(ref text); break;
            }

            Extensions.Broadcast(text, 1);
        }

        /// <summary>
        /// Updating variables before starting the game
        /// </summary>
        protected void UpdateWaitingState(ref string text)
        {
            _countdown = new TimeSpan(0, 0, 5);

            // Until ClassD is found, the game will not start
            if (_classD is null)
            {
                _eventState = EventState.ChooseClassD;
                return;
            }

            // Until Scientist is found, the game will not start
            if (_scientist is null)
            {
                _eventState = EventState.ChooseScientist;
                return;
            }

            // The game is starting
            _eventState = EventState.Playing;
        }

        /// <summary>
        /// Choosing a new player
        /// </summary>
        protected Player UpdateChoosePlayerState(ref string text, bool isClassD)
        {
            // Since we use the same method to select two states, we need these variables
            ushort value = 1;
            RoleTypeId role = RoleTypeId.ClassD;
            Player chosenPlayer;

            if (isClassD is not true)
            {
                value = 0;
                role = RoleTypeId.Scientist;
            }

            // We do a check for all players, weeding out unnecessary ones by roles
            foreach (Player player in Player.GetPlayers())
            {
                if (player.Role != role)
                    continue;

                // If the player is near the door, then we will teleport him
                if (Vector3.Distance(player.Position, _triggers.ElementAt(value).transform.position) <= 1f)
                {
                    chosenPlayer = player;
                    goto End;
                }
            }

            // Naturally, the player does not want to go to the door, so we wait for a while
            if (_countdown.TotalSeconds > 0)
                return null;

            // Teleporting a random player
            chosenPlayer = Player.GetPlayers().Where(r => r.Role == role).ToList().RandomItem();
            goto End;

        End:
            chosenPlayer.Position = _teleports.ElementAt(value).transform.position;
            _countdown = new TimeSpan(0, 0, 5);
            _eventState = EventState.Waiting;
            return chosenPlayer;
        }

        /// <summary>
        /// Game in process
        /// </summary>
        protected void UpdatePlayingState(ref string text)
        {
            // If the player has pressed the button, then proceed to the next state
            switch (_gunState)
            {
                case ShotgunState.ShootEnemy:
                    {
                        _animator.Play("Kill");
                        goto End;
                    }
                case ShotgunState.Suicide:
                    {
                        _animator.Play("Suicide");
                        goto End;
                    }
            }

            // We wait until the player clicks on the button
            if (_countdown.TotalSeconds > 0)
                return;

            // We forcibly take a shot
            _animator.Play("Suicide");
            goto End;

        End:
            _eventState = EventState.Shooting;
            return;
        }

        /// <summary>
        /// The player shot at another player
        /// </summary>
        protected void UpdateShootingState(ref string text)
        {
            float keyFrame = 10;
            if (_gunState is ShotgunState.Suicide)
            {
                keyFrame = 5;
            }

            // Проверяем фрейм анимации, при котором нужно убить игрока
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime == keyFrame)
            {
                bool rand = Random.Range(0, 2) == 1;
                if (rand is true)
                {
                    _classD.Kill("Ты проиграл");
                }
                else
                {
                    _scientist.Kill("Ты проиграл");
                }
            }

            // Пока анимация выстрела не кончится, то не переходим к концу
            // Kill / Suicide -> end of animation -> Idle
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _eventState = EventState.Finishing;
                _countdown = new TimeSpan(0, 0, 5);
            }
        }

        /// <summary>
        /// We check who survived and give him the opportunity to shoot
        /// </summary>
        protected void UpdateFinishingState(ref string text)
        {
            if (_countdown.TotalSeconds > 0)
                return;

            _isClassDMove = _classD.IsAlive ? true : false;
            _eventState = EventState.Waiting;
            _gunState = ShotgunState.None;
        }

        protected override void OnFinished()
        {
        }
    }
}
