using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using MEC;
using PlayerRoles;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using AutoEvent.API.RNG;
using AdminToys;
using Event = AutoEvent.Interfaces.Event;
using Random = UnityEngine.Random;
using CustomPlayerEffects;

namespace AutoEvent.Games.BuckshotRoulette;
public class Plugin : Event, IEventMap, IInternalEvent
{
    public override string Name { get; set; } = "Buckshot Roulette";
    public override string Description { get; set; } = "One-on-one battle in Russian roulette with shotguns";
    public override string Author { get; set; } = "KoT0XleB";
    public override string CommandName { get; set; } = "versus2";
    public override Version Version { get; set; } = new Version(1, 0, 0);
    [EventConfig]
    public Config Config { get; set; }
    [EventTranslation]
    public Translation Translation { get; set; }
    public MapInfo MapInfo { get; set; } = new MapInfo()
    { 
        MapName = "Buckshot",
        Position = new Vector3(0, 30, 30)
    };
    public SoundInfo SoundInfo { get; set; } = new SoundInfo()
    { 
        SoundName = "Knife.ogg", 
        Volume = 10
    };
    protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
    private EventHandler _eventHandler;

    // Variables that store the values of the players in the arena
    private Player _scientist;
    private Player _classD;
    private Player _choser;

    // Variables that store the values of MER objects
    private List<GameObject> _triggers;
    private List<GameObject> _teleports;
    private List<GameObject> _spawnpoints;
    private List<ShellClass> _shells;
    private GameObject _shotgunObject;

    // Other important variables
    private TimeSpan _countdown;
    private EventState _eventState;
    private ShotgunState _gunState;
    private Animator _animator;

    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler();
        EventManager.RegisterEvents(_eventHandler);
        Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
        Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
        Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
        Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
        Players.DropItem += _eventHandler.OnDropItem;
        Players.DropAmmo += _eventHandler.OnDropAmmo;
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
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _scientist = null;
        _classD = null;
        _choser = null;
        _shells = new();
        _triggers = new();
        _teleports = new();
        _spawnpoints = new();
        _shotgunObject = new();
        _eventState = 0;
        _gunState = 0;

        if (Config.Team1Loadouts == Config.Team2Loadouts)
        {
            DebugLogger.LogDebug("Warning: Teams should not have the same roles.", LogLevel.Warn, true);
        }

        foreach (GameObject block in MapInfo.Map.AttachedBlocks)
        {
            switch (block.name)
            {
                case "Trigger": _triggers.Add(block); break;
                case "Teleport": _teleports.Add(block); break;
                case "Spawnpoint": _spawnpoints.Add(block); break;
                case "Shell":
                    {
                        var prim = block.GetComponent<PrimitiveObjectToy>();
                        _shells.Add(new ShellClass() { Object = prim });
                    }
                    break;
                case "Shotgun":
                {
                    _shotgunObject = block;
                    _animator = _shotgunObject.GetComponent<Animator>();
                }
                break;
            }
        }

        var count = 0;
        foreach (Player player in Player.GetPlayers())
        {
            if (count % 2 == 0)     
            {              
                player.GiveLoadout(Config.Team1Loadouts);
                player.Position = _spawnpoints.ElementAt(0).transform.position;
            }
            else
            {
                player.GiveLoadout(Config.Team2Loadouts);
                player.Position = _spawnpoints.ElementAt(1).transform.position;
            }
            count++;
        }
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast(Translation.Start.
                Replace("{name}", Name).
                Replace("{time}", $"{time}"), 1);
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override bool IsRoundDone()
    {
        _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
        return !(Player.GetPlayers().Where(r => r.Role == RoleTypeId.ClassD).Count() > 0 && 
            Player.GetPlayers().Where(r => r.Role == RoleTypeId.Scientist).Count() > 0);
    }

    protected override void ProcessFrame()
    {
        StringaBuilder broadcast = new StringBuilder(Translation.Cycle);
        string text = string.Empty;
        string killerText = string.Empty;
        string targetText = string.Empty;

        switch (_eventState)
        {
            case EventState.Waiting: UpdateWaitingState(ref text); break;
            case EventState.ChooseClassD: _classD = UpdateChoosePlayerState(ref text, true); break;
            case EventState.ChooseScientist: _scientist = UpdateChoosePlayerState(ref text, false); break;
            case EventState.Playing: UpdatePlayingState(ref text, ref killerText, ref targetText); break;
            case EventState.Shooting: UpdateShootingState(ref text, ref killerText, ref targetText); break;
            case EventState.Finishing: UpdateFinishingState(ref text, ref killerText); break;
        }

        string scientistText = _scientist is not null && _scientist.IsAlive ? _scientist.Nickname : "Dead";
        string classdText = _classd is not null && _classd.IsAlive ? _classd.Nickname : "Dead";

        broadcast.Replace("{name}", Name).
            Replace("{state}", text).
            Replace("{time}", $"{_countdown.TotalSeconds}").
            Replace("{scientist}", scientistText).
            Replace("{classd}", classdText);

        foreach(Player player in Player.GetPlayers())
        {
            string task = string.Empty;
            if (player == _classD || player == _scientist)
            {
                if (player == _choser)
                {
                    task = killerText;
                }
                else 
                {
                    task = targetText";
                }
            }

            player.ClearBroadcasts();
            player.SendBroadcast(broadcast + $"\n{task}", 1);
        }
    }

    /// <summary>
    /// Updating variables before starting the game
    /// </summary>
    protected void UpdateWaitingState(ref string text)
    {
        _countdown = new TimeSpan(0, 0, 5);

        // Until Scientist is found, the game will not start
        if (_scientist is null)
        {
            text = Translation.ScientistNull;
            _eventState = EventState.ChooseScientist;
            return;
        }

        // Until ClassD is found, the game will not start
        if (_classD is null)
        {
            text = Translation.ScientistNull;
            _eventState = EventState.ChooseClassD;
            return;
        }

        // Generate new random shells
        if (_shells.Count(r => r.IsUsed == false) == 0)
        {
            foreach(var shell in _shells)
            {
                if (RNGGenerator.GetRandomNumber(0, 1) == 0)
                {
                    shell.Object.MaterialColor = Color.red;
                    shell.IsLoaded = true;
                }
                else
                {
                    shell.Object.MaterialColor = Color.cyan;
                    shell.IsLoaded = false;
                }
                shell.IsUsed = false;
            } 
        }

        // The game is starting
        text = Translation.Cycle;
        _choser ??= RNGGenerator.GetRandomNumber(0, 1) == 0 ? _scientist : _classD;
        _eventState = EventState.Playing;
    }

    /// <summary>
    /// Choosing a new player
    /// </summary>
    protected Player UpdateChoosePlayerState(ref string text, bool isScientist)
    {
        // Since we use the same method to select two states, we need these variables
        text = Translation.ScientistNull;
        ushort value = 0;
        RoleTypeId role = RoleTypeId.Scientist;
        Player chosenPlayer;

        if (isScientist is not true)
        {
            text = Translation.ClassDNull;
            value = 1;
            role = RoleTypeId.ClassD;
        }

        // We do a check for all players, weeding out unnecessary ones by roles
        foreach (Player player in Player.GetPlayers())
        {
            if (player.Role != role)
                continue;

            // If the player is near the door, then we will teleport him
            if (Vector3.Distance(player.Position, _triggers.ElementAt(value).transform.position) <= 1f)
            {
                chosenPlayer = player;
                goto End;
            }
        }

        // Naturally, the player does not want to go to the door, so we wait for a while
        if (_countdown.TotalSeconds > 0)
            return null;

        // Teleporting a random player
        chosenPlayer = Player.GetPlayers().Where(r => r.Role == role).ToList().RandomItem();
        goto End;

    End:
        chosenPlayer.Position = _teleports.ElementAt(value).transform.position;
        chosenPlayer.EffectsManager.EnableEffect<Ensnared>();
        _countdown = new TimeSpan(0, 0, 5);
        _eventState = EventState.Waiting;
        return chosenPlayer;
    }

    /// <summary>
    /// Game in process
    /// </summary>
    protected void UpdatePlayingState(ref string text, ref string killerText, ref string targetText)
    {
        text = Translation.Cycle;
        killerText = Translation.Killer;
        targetText = Translation.Target;

        // If the player has pressed the button, then proceed to the next state
        switch (_gunState)
        {
            case ShotgunState.ShootEnemy:
                {
                    _animator.Play("Kill");
                    goto End;
                }
            case ShotgunState.Suicide:
                {
                    _animator.Play("Suicide");
                    goto End;
                }
        }

        // We wait until the player clicks on the button
        if (_countdown.TotalSeconds > 0)
            return;

        // We forcibly take a shot
        _animator.Play("Suicide");
        _gunState = ShotgunState.Suicide;
        goto End;

    End:
        _eventState = EventState.Shooting;
        return;
    }

    /// <summary>
    /// The player shot at another player
    /// </summary>
    protected void UpdateShootingState(ref string text, ref string killerText, ref string targetText)
    {
        text = Translation.Cycle;
        killerText = Translation.Killer;
        targetText = Translation.Target;

        float framePercent = 0.5f; // Need check
        if (_gunState is ShotgunState.Suicide)
        {
            framePercent = 0.5f; // Need check
        }

        // Check the percentage of animation at which need to kill the player
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= framePercent && _scientist.IsAlive && _classD.IsAlive)
        {
            var item = _shells.Where(r => r.IsUsed == false).ToList().RandomItem();
            if (item.IsLoaded)
            {
                if (_choser == _classD)
                {
                    _scientist.Kill(Translation.Lose);
                }
                else
                {
                    _classD.Kill(Translation.Lose);
                }
            }
            else
            {
                if (_choser == _classD)
                {
                    _classD.Kill(Translation.Lose);
                }
                else
                {
                    _scientist.Kill(Translation.Lose);
                }
            }

            item.Object.MaterialColor = Color.grey;
            item.IsUsed = true;
        }

        // We are waiting for the end of the animation, after which we change it to default
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            // Give another player the ability to use a shotgun
            if (_scientist.IsAlive)
            {
                _choser = _scientist;
            }
            else if (_classD.IsAlive)
            {
                _choser = _classD;
            }
            else
            {
                _choser = null;
            }

            _animator.Play("Idle");
            _countdown = new TimeSpan(0, 0, 5);
            _eventState++;
        }
    }

    /// <summary>
    /// We check who survived and give him the opportunity to shoot
    /// </summary>
    protected void UpdateFinishingState(ref string text, ref string killerText)
    {
        text = Translation.Cycle;
        killerText = Translation.Defeat;

        if (_countdown.TotalSeconds > 0)
            return;

        _eventState = 0;
        _gunState = 0;
    }

    protected override void OnFinished()
    {
        string text = string.Empty;

        if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) == 0)
        {
            text = Translation.ScientistWin;
        }
        else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
        {
            text = Translation.ClassDWin;
        }
        else
        {
            text = Translation.Draw;
        }

        Extensions.Broadcast(text, 10);
    }
}
