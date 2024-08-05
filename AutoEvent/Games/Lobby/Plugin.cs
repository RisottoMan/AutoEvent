using System;
using PluginAPI.Events;
using System.Collections.Generic;
using AutoEvent.Interfaces;
using UnityEngine;
using System.Linq;
using PluginAPI.Core;

using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Lobby
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent, IHidden
    {
        public override string Name { get; set; } = "Lobby";
        public override string Description { get; set; } = "A lobby in which one quick player chooses a mini-game.";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "lobby";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "Lobby", 
            Position = new Vector3(76f, 1026.5f, -43.68f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "FireSale.ogg", 
            Volume = 10, 
            Loop = false 
        };
        private EventHandler _eventHandler;
        private LobbyState _state;
        private Player _chooser;
        private List<GameObject> _spawnpoints;
        private List<GameObject> _teleports;
        private Dictionary<GameObject, string> _platformes;
        private List<string> _eventList;
        private Event _newEvent;
        private TimeSpan _countdown;
        private bool _isLobbyEnded;

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _state = 0;
            _spawnpoints = new();
            _teleports = new();
            _platformes = new();
            _isLobbyEnded = false;
            _countdown = new TimeSpan(0, 0, 5);

            foreach (var obj in MapInfo.Map.AttachedBlocks)
            {
                try
                {
                    switch (obj.name)
                    {
                        case "Spawnpoint": _spawnpoints.Add(obj); break;
                        case "Teleport": _teleports.Add(obj); break;
                        case "Platform": _platformes.Add(obj, obj.transform.parent.name); break;
                    }
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"An error has occured at Lobby.OnStart()", LogLevel.Warn, true);
                    DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                }
            }

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = _spawnpoints.RandomItem().transform.position;
            }
        }

        protected override bool IsRoundDone()
        {
            DebugLogger.LogDebug($"Lobby state is {_state} and {(_newEvent is null ? "null" : _newEvent.Name)}", LogLevel.Debug);
            _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
            return _isLobbyEnded;
        }
        
        protected override void ProcessFrame()
        {
            string text = string.Empty;

            switch (_state)
            {
                case LobbyState.Waiting: LobbyWaitingState(ref text); break;
                case LobbyState.Running: LobbyRunningState(ref text); break;
                case LobbyState.Choosing: LobbyChoosingState(ref text); break;
                case LobbyState.Ending: LobbyEndingState(ref text); break;
            }

            text = Translation.GlobalMessage
                .Replace("{message}", text)
                .Replace("{count}", $"{Player.GetPlayers().Count()}");

            Extensions.Broadcast(text, 1);
        }

        /// <summary>
        /// We are waiting for the players to wake up
        /// </summary>
        /// <param name="text"></param>
        protected void LobbyWaitingState(ref string text)
        {
            text = Translation.GetReady;

            if (_countdown.TotalSeconds > 0)
                return;

            GameObject.Destroy(MapInfo.Map.AttachedBlocks.First(r => r.name == "Wall").gameObject);
            _countdown = new TimeSpan(0, 0, 10);
            _state++;
        }

        /// <summary>
        /// We are waiting for the player who will choose the next mini-game
        /// </summary>
        /// <param name="text"></param>
        protected void LobbyRunningState(ref string text)
        {
            text = Translation.Run;

            foreach (Player player in Player.GetPlayers().OrderBy(p => return Guid.NewGuid();))
            {
                if (Vector3.Distance(_teleports.ElementAt(0).transform.position, player.Position) < 1)
                {
                    _chooser = player;
                    goto End;
                }
            }

            if (_countdown.TotalSeconds > 0)
                return;

            _chooser = Player.GetPlayers().RandomItem();
            goto End;

        End:
            _chooser.Position = _teleports.ElementAt(1).transform.position;
            _countdown = new TimeSpan(0, 0, 15);
            _state++;
        }

        /// <summary>
        /// We are waiting for the chooser to choose a mini game
        /// </summary>
        /// <param name="text"></param>
        protected void LobbyChoosingState(ref string text)
        {
            text = Translation.Choosing.Replace("{nickName}", _chooser.Nickname);
            foreach (var platform in _platformes)
            {
                if (Vector3.Distance(platform.Key.transform.position, _chooser.Position) < 2)
                {
                    _newEvent = Event.GetEvent(platform.Value);
                    goto End;
                }
            }

            if (_countdown.TotalSeconds > 0)
                return;

            _newEvent = Event.GetEvent(_platformes.ToList().RandomItem().Value);
            goto End;

        End:
            _countdown = new TimeSpan(0, 0, 10);
            _state++;
        }

        /// <summary>
        /// We are waiting for some time after choosing a mini-game
        /// </summary>
        /// <param name="text"></param>
        protected void LobbyEndingState(ref string text)
        {
            text = Translation.FinishMessage
                .Replace("{nickName}", _chooser.Nickname)
                .Replace("{newName}", _newEvent.Name)
                .Replace("{count}", $"{Player.GetPlayers().Count()}");

            if (_countdown.TotalSeconds > 0)
                return;

            _newEvent = Event.GetEvent(_platformes.ToList().RandomItem().Value);
            _isLobbyEnded = true;
        }

        protected override void OnFinished()
        {
            // It is better to rewrite it, of course.
            Server.RunCommand($"ev run {_newEvent.CommandName}");
        }
    }
}
