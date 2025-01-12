using System;
using System.Collections.Generic;
using PlayerRoles;
using System.Linq;
using UnityEngine;
using AutoEvent.Interfaces;
using Exiled.API.Features;

namespace AutoEvent.Games.Football;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Football";
    public override string Description { get; set; } = "Score 3 goals to win";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "football";
    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "Football", 
        Position = new Vector3(0, 40f, 0f)
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "Football.ogg", 
        Volume = 5
    };
    protected override float FrameDelayInSeconds { get; set; } = 0.3f;
    private TimeSpan _remainingTime;
    private int _bluePoints;
    private int _redPoints;
    private GameObject _ball;
    private List<GameObject> _triggers;
    protected override void OnStart()
    {
        _remainingTime = new TimeSpan(0,0,Config.MatchDurationInSeconds);
        int count = 0;
        
        List<GameObject> spawnList = MapInfo.Map.AttachedBlocks.Where(r => r.name == "Spawnpoint").ToList();
        foreach (Player player in Player.List)
        {
            if (count % 2 == 0)
            {
                player.GiveLoadout(Config.BlueTeamLoadout);
                player.Position = spawnList.ElementAt(0).transform.position;
            }
            else
            {
                player.GiveLoadout(Config.OrangeTeamLoadout);
                player.Position = spawnList.ElementAt(1).transform.position;
            }
            count++;
        }
        
        _bluePoints = 0;
        _redPoints = 0;
        _ball = new GameObject();
        _triggers = new List<GameObject>();

        foreach (var gameObject in MapInfo.Map.AttachedBlocks)
        {
            switch(gameObject.name)
            {
                case "Trigger": { _triggers.Add(gameObject); } break;
                case "Ball": { _ball = gameObject; _ball.AddComponent<BallComponent>(); } break;
            }
        }
    }

    protected override bool IsRoundDone()
    {
        // Both teams have less than 3 points &&
        // The elapsed time is under 3 minutes &&
        // Both Teams have at least 1 player 
        return !(_bluePoints < Config.PointsToWin && _redPoints < Config.PointsToWin && 
                 EventTime.TotalSeconds < Config.MatchDurationInSeconds &&
                 Player.List.Count(r => r.IsNTF) > 0 &&
                 Player.List.Count(r => r.Role.Team == Team.ClassD) > 0);
    }

    protected override void ProcessFrame()
    {
        string time = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";
        foreach (Player player in Player.List)
        {
            var text = string.Empty;
            if (Vector3.Distance(_ball.transform.position, player.Position) < 2)
            {
                _ball.gameObject.TryGetComponent(out Rigidbody rig);
                rig.AddForce(player.ReferenceHub.transform.forward + new Vector3(0, 0.1f, 0), ForceMode.Impulse);
            }

            if (player.Role.Team == Team.FoundationForces)
            {
                text += Translation.BlueTeam;
            }
            else
            {
                text += Translation.RedTeam;
            }

            player.ClearBroadcasts();
            player.Broadcast(1, text + Translation.TimeLeft.
                Replace("{BluePnt}", $"{_bluePoints}").
                Replace("{RedPnt}", $"{_redPoints}").
                Replace("{time}", time));
        }

        if (Vector3.Distance(_ball.transform.position, _triggers.ElementAt(0).transform.position) < 3)
        {
            _ball.transform.position = MapInfo.Map.Position + new Vector3(0, 2.5f, 0);
            _ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _redPoints++;
        }

        if (Vector3.Distance(_ball.transform.position, _triggers.ElementAt(1).transform.position) < 3)
        {
            _ball.transform.position = MapInfo.Map.Position + new Vector3(0, 2.5f, 0);
            _ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _bluePoints++;
        }

        if (_ball.transform.position.y < MapInfo.Map.Position.y - 10f)
        {
            _ball.transform.position = MapInfo.Map.Position + new Vector3(0, 2.5f, 0);
            _ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
    }

    protected override void OnFinished()
    {
        if (_bluePoints > _redPoints)
        {
            Extensions.Broadcast($"{Translation.BlueWins}", 10);
        }
        else if (_redPoints > _bluePoints)
        {
            Extensions.Broadcast($"{Translation.RedWins}", 10);
        }
        else
        {
            Extensions.Broadcast($"{Translation.Draw.Replace("{BluePnt}", $"{_bluePoints}").Replace("{RedPnt}", $"{_redPoints}")}", 3);
        }
    }
}