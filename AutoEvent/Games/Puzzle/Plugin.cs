using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using UnityEngine;
using AdminToys;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using Mirror;
using Random = UnityEngine.Random;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Puzzle;
public class Plugin : Event, IEventSound, IEventMap
{
    public override string Name { get; set; } = "Puzzle";
    public override string Description { get; set; } = "Get up the fastest on the right color";
    public override string Author { get; set; } = "RisottoMan && Redforce";
    public override string CommandName { get; set; } = "puzzle";
    [EventConfig] public Config Config { get; set; }
    [EventTranslation] public Translation Translation { get; set; }

    public MapInfo MapInfo { get; set; } = new()
    {
        MapName = "Puzzle",
        Position = new Vector3(76f, 1026.5f, -43.68f)
    };

    public SoundInfo SoundInfo { get; set; } = new()
    {
        SoundName = "Puzzle.ogg",
        Volume = 15,
        Loop = true
    };
    private GridSelector GridSelector { get; set; }
    /// <summary>
    /// A local list of platforms that changes round to round.
    /// </summary>
    private List<GameObject> _listPlatforms;

    // Lists of game objects
    private List<GameObject> _colorIndicators;
    private List<GameObject> _nonFallingPlatforms;

    private int _stage;
    private EventState _eventState;
    private TimeSpan _countdown;
    private GridData _currentGridData;
    
    /// <summary>
    /// All platforms in the map.
    /// </summary>
    private Dictionary<ushort, GameObject> _platforms;
    private int _finaleStage => Config.Rounds;
    private float _speed;
    private float _timeDelay;
    private float _fallDelay;

    /// <summary>
    /// Interaction with players and objects before the start of the game
    /// </summary>
    protected override void OnStart()
    {
        _platforms = new();
        _colorIndicators = new();
        GameObject spawnpoint = new();
        GridSelector = new GridSelector(Config.PlatformsOnEachAxis, Config.PlatformsOnEachAxis, Config.Salt);
        
        foreach (GameObject block in MapInfo.Map.AttachedBlocks)
        {
            switch (block.name)
            {
                case "Lava":
                    block.AddComponent<LavaComponent>().StartComponent(this);
                    break;
                case "Indicator":
                    _colorIndicators.Add(block);
                    break;
                case "Spawnpoint":
                    spawnpoint = block;
                    break;
            }
        }

        GeneratePlatforms(Config.PlatformsOnEachAxis);
        foreach (Player player in Player.List)
        {
            player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
            player.Position = spawnpoint.transform.position;
        }
    }

    /// <summary>
    /// Generates platforms before starting the game
    /// </summary>
    private void GeneratePlatforms(int amountPerAxis = 5)
    {
        float areaSizeX = 20f;
        float areaSizeY = 20f;
        float sizeX = areaSizeX / amountPerAxis;
        float sizeY = areaSizeY / amountPerAxis;
        float startPosX = -(areaSizeX/2f) + sizeX / 2f;
        float startPosY = -(areaSizeY/2f) + sizeY / 2f;
        float breakSize = .2f;
        List<PlatformClass> platforms = new List<PlatformClass>();
        for (int x = 0; x < amountPerAxis; x++)
        {
            for (int y = 0; y < amountPerAxis; y++)
            {
                float posX = startPosX + (sizeX * x);
                float posY = startPosY + (sizeY * y);
                var plat = new PlatformClass(sizeX - breakSize, sizeY - breakSize, posX, posY);
                platforms.Add(plat);
            }
        }
        
        ushort id = 0;
        foreach (PlatformClass platform in platforms)
        {
            Vector3 position = MapInfo.Map.Position + new Vector3(platform.PositionX, 5.42f ,platform.PositionY);
            PrimitiveObject obj = ObjectSpawner.SpawnPrimitive(new PrimitiveSerializable()
            {
                PrimitiveType = PrimitiveType.Cube,
                Position = position,
                Color = "green"
            },
            position,
            Quaternion.identity,
            new Vector3(platform.X, 5f, platform.Y));

            NetworkServer.Spawn(obj.gameObject);
            _platforms.Add(id, obj.gameObject);
            id++;
        }
    }

    /// <summary>
    /// Broadcast before the start of the game
    /// </summary>
    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            string text = Translation.Start.Replace("{name}", Name).Replace("{time}", $"{time}");
            Extensions.Broadcast(text, 1);
            yield return Timing.WaitForSeconds(1f);
        }

        _eventState = 0;
        _stage = 1;
        _speed = 5;
        _timeDelay = 0.5f;
        _listPlatforms = _platforms.Values.ToList();
    }

    protected override bool IsRoundDone()
    {
        // Stage is smaller than the final stage &&
        // at least one player is alive.
        _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
        return !(_stage <= _finaleStage && Player.List.Count(r => r.IsAlive) > 0);
    }

    /// <summary>
    /// The logic of the mini-game
    /// </summary>
    protected override void ProcessFrame()
    {
        switch (_eventState)
        {
            case EventState.Waiting:
                UpdateWaitingState();
                break;
            case EventState.Starting:
                UpdateStartingState();
                break;
            case EventState.Falling:
                UpdateFallingState();
                break;
            case EventState.Returning:
                UpdateReturningState();
                break;
            case EventState.Ending:
                UpdateEndingState();
                break;
        }

        DebugLogger.LogDebug(_eventState.ToString());
        string text = Translation.Stage
            .Replace("{name}", Name)
            .Replace("{stageNum}", $"{_stage}")
            .Replace("{stageFinal}", $"{_finaleStage}")
            .Replace("{count}", $"{Player.List.Count(r => r.IsAlive)}");
        Extensions.Broadcast(text, 1);
    }

    /// <summary>
    /// Задаем начальные значения
    /// </summary>
    protected void UpdateWaitingState()
    {
        float selectionDelay = Config.SelectionTime.GetValue(_stage, 10, 0, 10);
        _fallDelay = Config.FallDelay.GetValue(_stage, 10, .3f,8);
        int spread = (int)Config.PlatformSpread.GetValue(_stage, Config.Rounds, 1, 3);
        float hueOffset = Config.HueDifficulty.GetValue(_stage, Config.Rounds, 0, 1);
        float satOffset = Config.SaturationDifficulty.GetValue(_stage, Config.Rounds, 0, 1);
        float vOffset = Config.VDifficulty.GetValue(_stage, Config.Rounds, 0, 1);
        int safePlatformCount = (int)Config.NonFallingPlatforms.GetValue(_stage, Config.Rounds, 1, 100);
        _currentGridData = GridSelector.SelectGridItem((byte)spread, true, null, hueOffset, satOffset, vOffset, safePlatformCount);
        DebugLogger.LogDebug($"Stage {_stage}: spread: {spread}, platformCount: {safePlatformCount}, hsv: {hueOffset}, {satOffset}, {vOffset}");
        var color = new Color(_currentGridData.SafePoints.First().Value.R / 255f, 
            _currentGridData.SafePoints.First().Value.G / 255f, 
            _currentGridData.SafePoints.First().Value.B / 255f);
        
        if (Config.UseRandomPlatformColors)
        {
            foreach (GameObject colorIndicator in _colorIndicators)
            {
                colorIndicator.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = color;
            }
        }
        
        _countdown = TimeSpan.FromSeconds((float)Math.Ceiling(selectionDelay / _timeDelay));
        FrameDelayInSeconds = _timeDelay;
        _eventState++;
    }

    /// <summary>
    /// The game is in an active process when the platforms change their color
    /// </summary>
    protected void UpdateStartingState() // скорость с которой вызывается StartingState слишком маленькая
    {
        foreach (var platform in _platforms.Values)
        {
            platform.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor =
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }

        if (_countdown.TotalSeconds > 0)
            return;

        // Change the color of those platforms that should fall to magenta
        _nonFallingPlatforms = new List<GameObject>();
        _listPlatforms = new List<GameObject>();
        foreach (var pnt in _currentGridData.SafePoints)
        {
            _nonFallingPlatforms.Add(_platforms[pnt.Key]);
        }
        try
        {
            foreach (var plat in _nonFallingPlatforms)
            {

                if (Config.UseRandomPlatformColors)
                {
                    plat.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = new Color(
                        _currentGridData.SafePoints.First().Value.R / 255f, 
                        _currentGridData.SafePoints.First().Value.G / 255f, 
                        _currentGridData.SafePoints.First().Value.B / 255f);
                }
                else
                {
                    plat.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.green;
                }
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Caught an exception while selecting colors. Exception: \n{e}");
        }

        foreach (var kvp in _platforms)
        {
            if (!_nonFallingPlatforms.Contains(kvp.Value))
            {
                if (Config.UseRandomPlatformColors)
                {
                    try
                    {
                        kvp.Value.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = new Color(
                            _currentGridData.Points[kvp.Key].R / 255f, 
                            _currentGridData.Points[kvp.Key].G / 255f,
                            _currentGridData.Points[kvp.Key].B / 255f);
                    }
                    catch (Exception e)
                    {
                        DebugLogger.LogDebug("Caught an exception while processing custom colors.", LogLevel.Warn, true);
                        DebugLogger.LogDebug($"{e}");
                        kvp.Value.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.magenta;
                        
                    }
                }
                else
                    kvp.Value.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.magenta;
                _listPlatforms.Add(kvp.Value);
            }
        }

        FrameDelayInSeconds = 1;
        _countdown = TimeSpan.FromSeconds(_fallDelay);
        _eventState++;
    }

    /// <summary>
    /// Platforms are falling down
    /// </summary>
    protected void UpdateFallingState()
    {
        if (_countdown.TotalSeconds > 0)
            return;

        foreach (var platform in _platforms.Values)
        {
            if (!_nonFallingPlatforms.Contains(platform))
            {
                platform.transform.position += Vector3.down * 5;
            }
        }
        
        _countdown = TimeSpan.FromSeconds(_fallDelay);
        _eventState++;
    }

    /// <summary>
    /// The platforms are coming back
    /// </summary>
    protected void UpdateReturningState()
    {
        if (_countdown.TotalSeconds > 0)
            return;

        foreach (var platform in _platforms.Values)
        {
            if (!_nonFallingPlatforms.Contains(platform))
            {
                platform.transform.position += Vector3.up * 5;
            }
        }
        
        _countdown = TimeSpan.FromSeconds(_speed);
        _eventState++;
    }

    /// <summary>
    /// Wait for a while
    /// </summary>
    protected void UpdateEndingState()
    {
        if (_countdown.TotalSeconds > 0)
            return;

        _speed -= 0.39f;
        _stage++;
        _timeDelay -= 0.039f;
        _eventState = 0;
    }

    protected override void OnFinished()
    {
        string text;
        int count = Player.List.Count(r => r.IsAlive);

        if (count < 1)
        {
            text = Translation.AllDied.Replace("{name}", Name);
        }
        else if (count == 1)
        {
            string nickname = Player.List.First(r => r.IsAlive).Nickname;
            text = Translation.Winner.Replace("{name}", Name).Replace("{winner}", nickname);
        }
        else
        {
            text = Translation.SomeSurvived.Replace("{name}", Name).Replace("{count}", $"{count}");
        }

        Extensions.Broadcast(text, 10);
    }
    
    protected override void OnCleanup()
    {
        foreach (var platform in this._platforms)
        {
            GameObject.Destroy(platform.Value);
        }
        base.OnCleanup();
    }
}