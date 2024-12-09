using System;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using AutoEvent.Events.Handlers;
using CommandSystem;
using MEC;

using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Vote;
public class Plugin : Event, IEventSound, IInternalEvent, IVote, IHiddenCommand
{
    public override string Name { get; set; } = "Vote";
    public override string Description { get; set; } = "Start voting for the mini-game";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "vote";
    public override Version Version { get; set; } = new Version(1, 1, 0);
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
    public Event NewEvent { get; set; }
    protected override float PostRoundDelay { get; set; } = 5f;
    private EventHandler _eventHandler { get; set; }
    public Dictionary<Player, bool> _voteList { get; set; }

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
        PostRoundDelay = Config.PostRoundDelayInSeconds;
        _voteList = new();

        foreach (Player player in Player.GetPlayers())
        {
            player.GiveLoadout(Config.PlayerLoadouts);
            _voteList.Add(player, false);
        }
    }

    protected override bool IsRoundDone()
    {
        return EventTime.Seconds == Config.VoteTimeInSeconds;
    }

    protected override void ProcessFrame()
    {
        var text = Translation.Cycle
            .Replace("{trueCount}", $"{_voteList.Count(r => r.Value)}")
            .Replace("{allCount}", $"{_voteList.Count}")
            .Replace("{newName}", NewEvent?.Name)
            .Replace("{time}", $"{(Config.VoteTimeInSeconds - EventTime.Seconds):00}");

        Extensions.Broadcast(text, 1);
    }

    protected override void OnFinished()
    {
        string results;
        int prosCount = _voteList.Count(r => r.Value);
        int consCount = _voteList.Count(r => !r.Value);
        
        if (prosCount > consCount)
        {
            results = Translation.StartResult.Replace("{newName}", NewEvent.Name);
            Timing.CallDelayed(PostRoundDelay + 0.1f, () =>
            {
                NewEvent.StartEvent();
                AutoEvent.ActiveEvent = NewEvent;
            });
        }
        else
        {
            results = Translation.NotStartResult.Replace("{newName}", NewEvent.Name);
        }

        var text = Translation.EndOfVoting
            .Replace("{trueCount}", $"{prosCount}")
            .Replace("{allCount}", $"{_voteList.Count}")
            .Replace("{result}", results);
        Extensions.Broadcast(text, 10);
    }
}