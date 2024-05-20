using System;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using AutoEvent.Events.Handlers;
using MEC;
using PluginAPI.Core;
using CommandSystem;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Vote
{
    public class Plugin : Event, IEventSound, IInternalEvent, IHiddenCommand
    {
        public override string Name { get; set; } = "Vote";
        public override string Description { get; set; } = "Start voting for the mini-game";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "vote";
        public override Version Version { get; set; } = new Version(1, 0, 0);
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
        public Dictionary<Player, bool> _voteList { get; set; }
        private int _voteTime = 30;
        public static string EventName { get; set; }
        public Event NewEvent { get; set; }

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Players.PlayerNoclip += _eventHandler.OnPlayerNoclip;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Players.PlayerNoclip -= _eventHandler.OnPlayerNoclip;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            _voteList = new();

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadouts);
                _voteList.Add(player, false);
            }
        }

        protected override bool IsRoundDone()
        {
            if (EventTime.Seconds != _voteTime) return false;
            else return true;
        }

        protected override void ProcessFrame()
        {
            var text = Translation.Cycle
                .Replace("{trueCount}", $"{_voteList.Count(r => r.Value == true)}")
                .Replace("{allCount}", $"{_voteList.Count}")
                .Replace("{newName}", NewEvent?.Name)
                .Replace("{time}", $"{(_voteTime - EventTime.Seconds):00}");

            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            string results;
            if (_voteList.Count(r => r.Value == true) > _voteList.Count(r => r.Value == false))
            {
                results = Translation.MinigameStart.Replace("{newName}", NewEvent.Name);

                // There is no way to change PostRoundDelay time to 5 second
                Timing.CallDelayed(10.1f, () =>
                {
                    Server.RunCommand($"ev run {NewEvent.CommandName}");
                });
            }
            else
            {
                results = Translation.MinigameNotStart.Replace("{newName}", NewEvent.Name);
            }

            var text = Translation.End
                .Replace("{trueCount}", _voteList.Count(r => r.Value == true).ToString())
                .Replace("{allCount}", _voteList.Count.ToString())
                .Replace("{result}", results);
            Extensions.Broadcast(text, 10);
        }
    }
}
