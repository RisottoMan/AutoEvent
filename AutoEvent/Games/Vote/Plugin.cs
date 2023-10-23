using System;
using PlayerRoles;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using AutoEvent.Events.Handlers;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Vote
{
    public class Plugin : Event, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = "Vote";
        public override string Description { get; set; } = "Start voting for the mini-game.";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "vote";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "FireSale.ogg", Volume = 10, Loop = false };
        private EventHandler EventHandler { get; set; }
        public Dictionary<Player, bool> _voteList { get; set; }
        private int _voteTime = 30;

        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler(this);
            EventManager.RegisterEvents(EventHandler);
            Players.PlayerNoclip += EventHandler.OnPlayerNoclip;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Players.PlayerNoclip -= EventHandler.OnPlayerNoclip;
            EventHandler = null;
        }

        protected override void OnStart()
        {
            _voteList = new();

            foreach (Player player in Player.GetPlayers())
            {
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
            var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
            var time = $"{(_voteTime - EventTime.Seconds):00}";

            Extensions.Broadcast($"Vote: Press [Alt] Pros or twice Cons\n" +
                $"{_voteList.Count(r => r.Value == true)} of {_voteList.Count} players for Mini-Game %Name%\n" +
                $"{time} seconds left!", 1);
        }

        protected override void OnFinished()
        {
            string results;
            if (_voteList.Count(r => r.Value == true) > _voteList.Count(r => r.Value == false))
            {
                results = "Mini-Game %Name% will start soon.";
                // timing -> ev run name
                // idea: do I need to make an auto discord message about voting? About launching a mini-game?
            }
            else
            {
                results = "Mini-Game %Name% will not start.";
                // nothing 
            }

            Extensions.Broadcast($"Vote: End of voting\n" +
                $"{_voteList.Count(r => r.Value == true)} of {_voteList.Count} players\n" +
                $"{results}", 10);
        }
    }
}
