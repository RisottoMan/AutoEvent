using System;
using PluginAPI.Events;
using System.Collections.Generic;
using AutoEvent.Interfaces;
using UnityEngine;
using System.Linq;
using CommandSystem;
using PluginAPI.Core;
using MEC;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Lobby
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent, IHiddenCommand
    {
        public override string Name { get; set; } = "Lobby";
        public override string Description { get; set; } = "A lobby in which one quick player chooses a mini-game.";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "lobby";
        public override Version Version { get; set; } = new Version(1, 0, 0);
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
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        private EventHandler _eventHandler { get; set; }
        LobbyState _state { get; set; }
        Player _chooser { get; set; }
        List<GameObject> _spawnpoints { get; set; }
        List<GameObject> _teleports { get; set; }
        Dictionary<GameObject, string> _platformes { get; set; }
        public List<string> _eventList { get; set; }
        public Event NewEvent { get; set; }

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
            DebugLogger.LogDebug($"Lobby is started");
            _state = LobbyState.Waiting;

            InitGameObjects();

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = _spawnpoints.RandomItem().transform.position;
            }
        }

        protected void InitGameObjects()
        {
            _spawnpoints = new();
            _teleports = new();
            _platformes = new();
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
        }

        protected override bool IsRoundDone()
        {
            DebugLogger.LogDebug($"Lobby state is {_state} and {(NewEvent is null ? "null" : NewEvent.Name)}", LogLevel.Debug);

            if (_state != LobbyState.Ending)
                return false;

            return true;
        }
        
        protected override void ProcessFrame()
        {
            string message = Translation.GetReady;

            if (_state == LobbyState.Waiting && EventTime.TotalSeconds >= 5)
            {
                GameObject.Destroy(MapInfo.Map.AttachedBlocks.First(r => r.name == "Wall").gameObject);
                EventTime = new();
                _state = LobbyState.Running;
            }

            if (_state == LobbyState.Running)
            {
                message = Translation.Run;
                if (EventTime.TotalSeconds <= 10)
                {
                    foreach (Player player in Player.GetPlayers())
                    {
                        if (Vector3.Distance(_teleports.ElementAt(0).transform.position, player.Position) < 1)
                        {
                            _chooser = player;
                            _chooser.Position = _teleports.ElementAt(1).transform.position;
                            EventTime = new();
                            _state = LobbyState.Choosing;
                        }
                    }
                }
                else
                {
                    _chooser = Player.GetPlayers().RandomItem();
                    _chooser.Position = _teleports.ElementAt(1).transform.position;
                    EventTime = new();
                    _state = LobbyState.Choosing;
                }
            }

            if (_state == LobbyState.Choosing)
            {
                message = Translation.Choosing.Replace("{nickName}", _chooser.Nickname);
                if (EventTime.TotalSeconds <= 15)
                {
                    foreach (var platform in _platformes)
                    {
                        if (Vector3.Distance(platform.Key.transform.position, _chooser.Position) < 2)
                        {
                            NewEvent = Event.GetEvent(platform.Value);
                            _state = LobbyState.Ending;
                        }
                    }
                }
                else
                {
                    NewEvent = Event.GetEvent(_platformes.ToList().RandomItem().Value);
                    _state = LobbyState.Ending;
                }
            }

            var text = Translation.GlobalMessage
                .Replace("{message}", message)
                .Replace("{count}", Player.GetPlayers().Count().ToString());
            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            DebugLogger.LogDebug($"Lobby is finished");

            var text = Translation.FinishMessage
                .Replace("{nickName}", _chooser.Nickname)
                .Replace("{newName}", NewEvent.Name)
                .Replace("{count}", Player.GetPlayers().Count().ToString());
            Extensions.Broadcast(text, 10);

            Timing.CallDelayed(10.1f, () =>
            {
                Server.RunCommand($"ev run {NewEvent.CommandName}");
            });
        }
    }
}

enum LobbyState
{
    Waiting,
    Running,
    Choosing,
    Ending
}
