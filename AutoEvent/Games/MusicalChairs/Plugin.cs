using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using AdminToys;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.MusicalChairs;
public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
{
    public override string Name { get; set; } = "Musical Chairs";
    public override string Description { get; set; } = "Competition with other players for free chairs to funny music";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "chair";
    public override Version Version { get; set; } = new Version(1, 1, 0);
    [EventConfig]
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "MusicalChairs",
        Position = new Vector3(0, 0, 30),
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "MusicalChairs.ogg",
        Volume = 10,
        Loop = false
    };
    private EventHandler _eventHandler;
    private EventState _eventState;
    private GameObject _parentPlatform;
    private Dictionary<Player, PlayerClass> _playerDict;
    private TimeSpan _countdown;
    
    internal List<GameObject> Platforms;
    
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        EventManager.RegisterEvents(_eventHandler);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
        Players.PlayerDamage += _eventHandler.OnDamage;
        Players.UsingStamina += _eventHandler.OnUsingStamina;
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
        Players.PlayerDamage -= _eventHandler.OnDamage;
        Players.UsingStamina -= _eventHandler.OnUsingStamina;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _eventState = 0;
        _countdown = new TimeSpan(0, 0, 5);
        List<GameObject> spawnpoints = new List<GameObject>();

        foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
        {
            switch(gameObject.name)
            {
                case "Spawnpoint": spawnpoints.Add(gameObject); break;
                case "Cylinder-Parent": _parentPlatform = gameObject; break;
            }
        }

        int count = Player.GetPlayers().Count > 40 ? 40 : Player.GetPlayers().Count - 1;
        Platforms = Functions.GeneratePlatforms(count, _parentPlatform, MapInfo.Position);
        
        foreach (Player player in Player.GetPlayers())
        {
            player.GiveLoadout(Config.PlayerLoadout);
            player.Position = spawnpoints.RandomItem().transform.position;
        }
        
        _playerDict = new();
        foreach (Player player in Player.GetPlayers())
        {
            PlayerClass playerClass = new PlayerClass()
            {
                Angle = 0,
                IsStandUpPlatform = false
            };

            _playerDict.Add(player, playerClass);
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            string text = Translation.Start.Replace("{time}", time.ToString());
            Extensions.Broadcast(text, 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override bool IsRoundDone()
    {
        _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero; 
        return !(Player.GetPlayers().Count(r => r.IsAlive) > 1);
    }
    protected override float FrameDelayInSeconds { get; set; } = 0.5f;
    protected override void ProcessFrame()
    {
        string text = string.Empty;
        switch (_eventState)
        {
            case EventState.Waiting: UpdateWaitingState(ref text); break;
            case EventState.Playing: UpdatePlayingState(ref text); break;
            case EventState.Stopping: UpdateStoppingState(ref text); break;
            case EventState.Ending: UpdateEndingState(ref text); break;
        }

        Extensions.Broadcast(Translation.Cycle.
            Replace("{name}", Name).
            Replace("{state}", text).
            Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsAlive)}"), 1);
    }

    /// <summary>
    /// The state in which we set the initial values for the new game
    /// </summary>
    /// <param name="text"></param>
    protected void UpdateWaitingState(ref string text)
    {
        text = Translation.RunDontTouch;

        if (_countdown.TotalSeconds > 0)
            return;

        foreach (var value in _playerDict.Values)
        {
            value.Angle = 0;
            value.IsStandUpPlatform = false;
        }
        
        _countdown = new TimeSpan(0, 0, UnityEngine.Random.Range(2, 10));
        _eventState++;
    }

    /// <summary>
    /// Game cycle in which we check that the player runs around the center and does not touch the platforms
    /// </summary>
    /// <param name="text"></param>
    protected void UpdatePlayingState(ref string text)
    {
        text = Translation.RunDontTouch;

        foreach (Player player in Player.GetPlayers())
        {
            float playerAngle = 180f + Mathf.Rad2Deg * (Mathf.Atan2(player.Position.z - MapInfo.Position.z, player.Position.x - MapInfo.Position.x));

            // The player can run in any direction. The main thing is that the angle changes and is not the same
            if (Mathf.Approximately(_playerDict[player].Angle, playerAngle))
            {
                Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                player.Kill(Translation.StopRunning);
            }
            else
            {
                _playerDict[player].Angle = playerAngle;
            }

            // If the player touches the platform, it will explode || Layer mask is 0 for primitives
            if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 3, 1 << 0))
            {
                if (!Platforms.Contains(hit.collider.gameObject))
                    continue;

                if (hit.collider.GetComponent<PrimitiveObjectToy>())
                {
                    Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                    player.Kill(Translation.TouchAhead);
                }
            }
        }

        if (_countdown.TotalSeconds > 0)
            return;

        foreach (var platform in Platforms)
        {
            platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.black;
        }

        Extensions.PauseAudio();
        _countdown = new TimeSpan(0, 0, 3);
        _eventState++;
    }

    /// <summary>
    /// The game stops and the players have to stand on the platforms
    /// </summary>
    /// <param name="text"></param>
    protected void UpdateStoppingState(ref string text)
    {
        text = Translation.StandFree;
        
        foreach (Player player in Player.GetPlayers())
        {
            // Player is not contains in _playerDict
            if (!_playerDict.TryGetValue(player, out var playerClass))
                continue;
            
            // The player has already stood on the platform
            if (playerClass.IsStandUpPlatform)
                continue;

            // Layer mask is 0 for primitives
            if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 3, 1 << 0))
            {
                if (!Platforms.Contains(hit.collider.gameObject))
                    continue;

                if (hit.collider.TryGetComponent(out PrimitiveObjectToy objectToy))
                {
                    if (objectToy.NetworkMaterialColor == Color.black)
                    {
                        objectToy.NetworkMaterialColor = Color.red;
                        playerClass.IsStandUpPlatform = true;
                    }
                }
            }
        }
        
        if (_countdown.TotalSeconds > 0)
            return;

        _countdown = new TimeSpan(0, 0, 3);
        _eventState++;
    }

    /// <summary>
    /// Kill players who did not manage to stand on the platforms
    /// </summary>
    /// <param name="text"></param>
    protected void UpdateEndingState(ref string text)
    {
        text = Translation.StandFree;

        foreach (Player player in Player.GetPlayers().Where(r => r.IsAlive))
        {
            // Kill the players who didn't get up to platform
            if (!_playerDict[player].IsStandUpPlatform)
            {
                Extensions.GrenadeSpawn(0.1f, player.Position, 0.1f);
                player.Kill(Translation.NoTime);
            }
        }

        if (_countdown.TotalSeconds > 0)
            return;

        Extensions.ResumeAudio();
        _countdown = new TimeSpan(0, 0, 3);
        _eventState = 0;

        foreach (var platform in Platforms)
        {
            platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.yellow;
        }
    }

    protected override void OnFinished()
    {
        string text = string.Empty;
        int count = Player.GetPlayers().Count(r => r.IsAlive);

        if (count > 1)
        {
            text = Translation.MorePlayers.Replace("{name}", Name);
        }
        else if (count == 1)
        {
            Player winner = Player.GetPlayers().First(r => r.IsAlive);
            text = Translation.Winner.Replace("{name}", Name).Replace("{winner}", winner.Nickname);
        }
        else
        {
            text = Translation.AllDied.Replace("{name}", Name);
        }

        Extensions.Broadcast(text, 10);
    }

    protected override void OnCleanup()
    {
        foreach (GameObject platform in Platforms)
        {
            GameObject.Destroy(platform);
        }
    }
}