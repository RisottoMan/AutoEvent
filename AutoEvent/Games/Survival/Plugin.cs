using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Interfaces;
using Exiled.API.Features;

namespace AutoEvent.Games.Survival;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Zombie Survival";
    public override string Description { get; set; } = "Humans surviving from zombies";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "zombie2";
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "Survival", 
        Position = new Vector3(0f, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "Survival.ogg",
        Volume = 10,
        Loop = false
    };
    private EventHandler _eventHandler { get; set; }
    private GameObject _teleport;
    private GameObject _teleport1;
    private TimeSpan _remainingTime;
    internal Player FirstZombie;
    internal List<GameObject> SpawnList;

    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Joined += _eventHandler.OnJoined;
        Exiled.Events.Handlers.Player.Dying += _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurting;
    }

    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Joined -= _eventHandler.OnJoined;
        Exiled.Events.Handlers.Player.Dying -= _eventHandler.OnDying;
        Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurting;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _remainingTime = new TimeSpan(0, 5, 0);
        Server.FriendlyFire = false;
        
        SpawnList = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList();
        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.PlayerLoadouts);
            player.Position = SpawnList.RandomItem().transform.position;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (float _time = 20; _time > 0; _time--)
        {
            Extensions.Broadcast(Translation.SurvivalBeforeInfection.Replace("{name}", Name).Replace("{time}", $"{_time}"), 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override void CountdownFinished()
    {
        Extensions.PlayAudio("Zombie2.ogg", 7, true);

        List<Player> players = Config.Zombies.GetPlayers(true);
        foreach (Player player in players)
        {
            DebugLogger.LogDebug($"Making player {player.Nickname} a zombie.");
            player.GiveLoadout(Config.ZombieLoadouts);
            Extensions.PlayPlayerAudio(SoundInfo.AudioPlayer, player, Config.ZombieScreams.RandomItem(), 15);
            
            if (Player.List.Count(r => r.IsScp) == 1)
            {
                if (FirstZombie is not null)
                    continue;
                
                FirstZombie = player;
            }
        }

        _teleport = MapInfo.Map.AttachedBlocks.First(x => x.name == "Teleport");
        _teleport1 = MapInfo.Map.AttachedBlocks.First(x => x.name == "Teleport1");
    }

    protected override bool IsRoundDone()
    {
        // At least 1 human player &&
        // At least 1 scp player &&
        // round time under 5 minutes (+ countdown)
        bool a = Player.List.Any(ply => ply.HasLoadout(Config.PlayerLoadouts));
        bool b = Player.List.Any(ply => ply.HasLoadout(Config.ZombieLoadouts));
        bool c = EventTime.TotalSeconds < Config.RoundDurationInSeconds;
        return !(a && b && c);
    }

    protected override void ProcessFrame()
    {
        var text = Translation.SurvivalAfterInfection;
        
        text = text.Replace("{name}", Name);
        text = text.Replace("{humanCount}", Player.List.Count(r => r.IsHuman).ToString());
        text = text.Replace("{time}", $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}");

        foreach (var player in Player.List)
        {
            player.ClearBroadcasts();
            player.Broadcast(1, text);

            if (Vector3.Distance(player.Position, _teleport.transform.position) < 1)
            {
                player.Position = _teleport1.transform.position;
            }
        }

        _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        string musicName = "HumanWin.ogg";

        if (Player.List.Count(r => r.IsHuman) == 0)
        {
            text = Translation.SurvivalZombieWin;
            musicName = "ZombieWin.ogg";
        }
        else if (Player.List.Count(r => r.IsScp) == 0)
        {
            text = Translation.SurvivalHumanWin;
        }
        else
        {
            text = Translation.SurvivalHumanWinTime;
        }

        Extensions.PauseAudio(SoundInfo.AudioPlayer);
        Extensions.PlayAudio(musicName, 7, false);
        Extensions.Broadcast(text, 10);
    }
}