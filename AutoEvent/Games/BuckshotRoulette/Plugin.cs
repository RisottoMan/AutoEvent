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
    public class Plugin : Event, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Buckshot Roulette";
        public override string Description { get; set; } = "One-on-one battle in Russian roulette with shotguns";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "versus2";
        public override Version Version { get; set; } = new Version(1, 0, 2);
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
            Volume = 10
        };
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        private EventHandler _eventHandler;
        private Player _scientist;
        private Player _classD;
        private List<GameObject> _triggers;
        private List<GameObject> _teleports;
        private List<GameObject> _spawnpoints;
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
            _spawnpoints = new();
            _shotgunObject = new();
            _eventState = 0;
            _gunState = 0;
            _isClassDMove = true;

            if (Config.Team1Loadouts == Config.Team2Loadouts)
            {
                DebugLogger.LogDebug("Warning: Teams should not have the same roles.", LogLevel.Warn, true);
            }

            foreach (GameObject block in MapInfo.Map.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "Trigger": _triggers.Add(block); break;
                    case "Teleport": _teleports.Add(block); break;
                    case "Spawnpoint": _spawnpoints.Add(block); break;
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
                    player.Position = _spawnpoints.ElementAt(0).transform.position;
                }
                else
                {
                    player.GiveLoadout(Config.Team2Loadouts);
                    player.Position = _spawnpoints.ElementAt(1).transform.position;
                }
                count++;
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{Name}\nИгроки приготовьтесь зайти на арену\nРусская рулетка начнется через {time} секунд", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            DebugLogger.LogDebug("BroadcastStartCountdown", LogLevel.Debug, true);
        }

        protected override bool IsRoundDone()
        {
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

            // Until Scientist is found, the game will not start
            if (_scientist is null)
            {
                _eventState = EventState.ChooseScientist;
                return;
            }

            // Until ClassD is found, the game will not start
            if (_classD is null)
            {
                _eventState = EventState.ChooseClassD;
                return;
            }

            // The game is starting
            _eventState = EventState.Playing;
        }

        /// <summary>
        /// Choosing a new player
        /// </summary>
        protected Player UpdateChoosePlayerState(ref string text, bool isScientist)
        {
            text = $"{Name}\nИгроки из команды Ученых зайдите на арену\nУ вас осталось {_countdown.TotalSeconds} секунд";
            // Since we use the same method to select two states, we need these variables
            ushort value = 0;
            RoleTypeId role = RoleTypeId.Scientist;
            Player chosenPlayer;

            if (isScientist is not true)
            {
            text = $"{Name}\nИгроки из команды Д-Класс зайдите на арену\nУ вас осталось {_countdown.TotalSeconds} секунд";
                value = 1;
                role = RoleTypeId.ClassD;
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
            text = $"{Name}\n{_scientist} VS {_classD}\nНажмите на кнопку для выбора в течении {_countdown.TotalSeconds} секунд";
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
            text = $"{Name}\n{_scientist} VS {_classD}\nИгрок выбрал выстрелить в противника";
            float framePercent = 0.5f;
            if (_gunState is ShotgunState.Suicide)
            {
            text = $"{Name}\n{_scientist} VS {_classD}\nИгрок выбрал выстрелить в себя";
                framePercent = 0.3f;
            }

            // Проверяем фрейм анимации, при котором нужно убить игрока
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime == framePercent)
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
            if (_isClassDMove is true) 
            {
                if (!_classD.IsAlive)
                {
                    text = $"{Name}\nИгрок {_classD} застрелился\nИгрок {_scientist} остался в живых";
                }
                else
                {
                    text = $"{Name}\nИгрок {_classD} застрелил {_scientist}\nИгрок {_classD} остался в живых";
                }
            }
            else
            {
                if (!_scientist.IsAlive)
                {
                    text = $"{Name}\nИгрок {_scientist} застрелился\nИгрок {_classD} остался в живых";
                }
                else
                {
                    text = $"{Name}\nИгрок {_scientist} застрелил {_classD}\nИгрок {_scientist} остался в живых";
                }
            }

            if (_countdown.TotalSeconds > 0)
                return;

            _isClassDMove = _classD.IsAlive ? true : false;
            _eventState = EventState.Waiting;
            _gunState = ShotgunState.None;
        }

        protected override void OnFinished()
        {
            string text = string.Empty;

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) == 0)
            {
                text = $"{Name}\nКоманда Д-Класс победила Ученых";
            }
            else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                text = $"{Name}\nКоманда Ученых победила Д-Класс";
            }
            else
            {
                text = $"{Name}\nНичья\nКак можно было так проиграть?!";
            }

            Extensions.Broadcast(text, 10);
        }
    }
}
