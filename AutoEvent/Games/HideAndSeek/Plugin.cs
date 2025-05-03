using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Events;
using UnityEngine;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace AutoEvent.Games.HideAndSeek;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Tag";
    public override string Description { get; set; } = "We need to catch up with all the players on the map";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "tag";
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "HideAndSeek", 
        Position = new Vector3(0, 30, 30)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "HideAndSeek.ogg", 
        Volume = 5
    };
    private EventHandler _eventHandler;
    private TimeSpan _countdown;
    private EventState _eventState;
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurting;
        Exiled.Events.Handlers.Item.ChargingJailbird += _eventHandler.OnJailbirdCharge;
    }
    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurting;
        Exiled.Events.Handlers.Item.ChargingJailbird -= _eventHandler.OnJailbirdCharge;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _eventState = 0;
        List<GameObject> spawnpoints = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList();
        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.PlayerLoadouts);
            player.Position = spawnpoints.RandomItem().transform.position;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (float _time = 15; _time > 0; _time--)
        {
            Extensions.Broadcast(Translation.Broadcast.Replace("{time}", $"{_time}"), 1);

            yield return Timing.WaitForSeconds(1f);
            EventTime += TimeSpan.FromSeconds(1f);
        }
    }

    protected override bool IsRoundDone()
    {
        _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
        return !(Player.List.Count(ply => ply.IsAlive) > 1);
    }

    protected override void ProcessFrame()
    {
        string text = string.Empty;
        switch (_eventState)
        {
            case EventState.SelectPlayers: SelectPlayers(ref text); break;
            case EventState.TagPeriod: UpdateTagPeriod(ref text); break;
            case EventState.KillTaggers: KillTaggers(ref text); break;
            case EventState.PlayerBreak: UpdatePlayerBreak(ref text); break;
        }

        Extensions.Broadcast(text, 1);
    }

    /// <summary>
    /// Choosing the player(s) who will catch up with other players
    /// </summary>
    protected void SelectPlayers(ref string text)
    {
        text = Translation.Broadcast.Replace("{time}", $"{_countdown.TotalSeconds}");
        List<Player> playersToChoose = Player.List.Where(x => x.IsAlive).ToList();
        foreach (Player ply in Config.TaggerCount.GetPlayers(true, playersToChoose))
        {
            ply.GiveLoadout(Config.TaggerLoadouts);
            if (ply.CurrentItem == null)
            {
                ply.CurrentItem = ply.AddItem(Config.TaggerWeapon);
            }
        }

        if (Player.List.Count(ply => ply.HasLoadout(Config.PlayerLoadouts)) <= Config.PlayersRequiredForBreachScannerEffect)
        {
            foreach (Player player in Player.List.Where(ply => ply.HasLoadout(Config.PlayerLoadouts)))
            {
                player.EnableEffect(EffectType.Scanned, 255);
            }
        }

        _countdown = new TimeSpan(0, 0, Config.TagDuration);
        _eventState++;
    }

    /// <summary>
    /// Just waiting N seconds until the time runs out
    /// </summary>
    protected void UpdateTagPeriod(ref string text)
    {
        text = Translation.Cycle.Replace("{time}", $"{_countdown.TotalSeconds}");

        if (_countdown.TotalSeconds <= 0)
            _eventState++;
    }

    /// <summary>
    /// Kill players who are taggers.
    /// </summary>
    protected void KillTaggers(ref string text)
    {
        text = Translation.Cycle.Replace("{time}", $"{_countdown.TotalSeconds}");

        foreach (Player player in Player.List)
        {
            if (player.Items.Any(r => r.Type == Config.TaggerWeapon))
            {
                player.ClearInventory();
                player.Hurt(200, Translation.Hurt);
            }
        }

        _countdown = new TimeSpan(0, 0, Config.BreakDuration);
        _eventState++;
    }

    /// <summary>
    /// Wait for N seconds before choosing next batch.
    /// </summary>
    protected void UpdatePlayerBreak(ref string text)
    {
        text = Translation.Broadcast.Replace("{time}", $"{_countdown.TotalSeconds}");

        if (_countdown.TotalSeconds <= 0)
            _eventState = 0;
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        if (Player.List.Count(r => r.IsAlive) >= 1)
        {
            text = Translation.OnePlayer
                .Replace("{winner}", Player.List.First(r => r.IsAlive).Nickname)
                .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");
        }
        else
        {
            text = Translation.AllDie.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");
        }

        Extensions.Broadcast(text, 10);
    }
}