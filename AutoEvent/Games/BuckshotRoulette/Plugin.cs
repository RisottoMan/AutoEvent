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
using CustomPlayerEffects;
using System.Text;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.BuckshotRoulette;
public class Plugin : Event, IEventMap, IInternalEvent, IHidden
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
    internal Player Choser;

    // Variables that store the values of MER objects
    private List<GameObject> _triggers;
    private List<GameObject> _teleports;
    private List<GameObject> _spawnpoints;
    private List<ShellClass> _shells;
    private GameObject _shotgunObject;

    // Other important variables
    private TimeSpan _countdown;
    private Animator _animator;
    internal EventState EventState;
    internal ShotgunState GunState;

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
        Players.PickUpItem += _eventHandler.OnPickupItem;
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
        Players.PickUpItem -= _eventHandler.OnPickupItem;
        _eventHandler = null;
    }

    protected override void OnStart()
    {
        _scientist = null;
        _classD = null;
        Choser = null;
        _shells = new();
        _triggers = new();
        _teleports = new();
        _spawnpoints = new();
        _shotgunObject = new();
        EventState = 0;
        GunState = 0;

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
                        _shells.Add(new ShellClass() { Object = prim, IsUsed = true });
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
        StringBuilder broadcast = new StringBuilder(Translation.Cycle);
        string text = string.Empty;
        string killerText = string.Empty;
        string targetText = string.Empty;

        switch (EventState)
        {
            case EventState.Waiting: UpdateWaitingState(ref text); break;
            case EventState.ChooseClassD: _classD = UpdateChoosePlayerState(ref text, true); break;
            case EventState.ChooseScientist: _scientist = UpdateChoosePlayerState(ref text, false); break;
            case EventState.Playing: UpdatePlayingState(ref text, ref killerText, ref targetText); break;
            case EventState.Shooting: UpdateShootingState(ref text, ref killerText, ref targetText); break;
            case EventState.Finishing: UpdateFinishingState(ref text, ref killerText, ref targetText); break;
        }

        string scientistText = _scientist is not null && _scientist.IsAlive ? _scientist.Nickname : Translation.DeadBroadcast;
        string classdText = _classD is not null && _classD.IsAlive ? _classD.Nickname : Translation.DeadBroadcast;

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
                if (player == Choser)
                {
                    task = killerText;
                }
                else 
                {
                    task = targetText;
                }
                task = task.Replace("{time}", $"{_countdown.TotalSeconds}");
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
            text = Translation.WaitingScientist;
            EventState = EventState.ChooseScientist;
            return;
        }

        // Until ClassD is found, the game will not start
        if (_classD is null)
        {
            text = Translation.WaitingClassD;
            EventState = EventState.ChooseClassD;
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
        Choser ??= RNGGenerator.GetRandomNumber(0, 1) == 0 ? _scientist : _classD;
        EventState = EventState.Playing;
    }

    /// <summary>
    /// Choosing a new player
    /// </summary>
    protected Player UpdateChoosePlayerState(ref string text, bool isScientist)
    {
        // Since we use the same method to select two states, we need these variables
        text = Translation.WaitingScientist;
        ushort value = 0;
        RoleTypeId role = RoleTypeId.Scientist;
        Player chosenPlayer;

        if (isScientist is not true)
        {
            text = Translation.WaitingClassD;
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
        EventState = EventState.Waiting;
        return chosenPlayer;
    }

    /// <summary>
    /// Game in process
    /// </summary>
    protected void UpdatePlayingState(ref string text, ref string killerText, ref string targetText)
    {
        text = Translation.Cycle;
        killerText = Translation.PressButton;
        targetText = Translation.WaitAction;

        // If the player has pressed the button, then proceed to the next state
        switch (GunState)
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
        GunState = ShotgunState.Suicide;
        goto End;

    End:
        EventState++;
        return;
    }

    /// <summary>
    /// The player shot at another player
    /// </summary>
    protected void UpdateShootingState(ref string text, ref string killerText, ref string targetText)
    {
        text = Translation.Cycle;
        killerText = Translation.ChoiceMade;
        targetText = Translation.ChoiceMade;

        float framePercent = 0.5f; // Need check
        if (GunState is ShotgunState.Suicide)
        {
            framePercent = 0.5f; // Need check
        }

        // Check the percentage of animation at which need to kill the player
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= framePercent && _scientist.IsAlive && _classD.IsAlive)
        {
            var item = _shells.Where(r => r.IsUsed == false).ToList().RandomItem();
            if (item.IsLoaded)
            {
                if (Choser == _classD)
                {
                    _scientist.Kill(Translation.KillMessage);
                }
                else
                {
                    _classD.Kill(Translation.KillMessage);
                }
            }
            else
            {
                if (Choser == _classD)
                {
                    _classD.Kill(Translation.KillMessage);
                }
                else
                {
                    _scientist.Kill(Translation.KillMessage);
                }
            }

            item.Object.MaterialColor = Color.grey;
            item.IsUsed = true;
        }

        // We are waiting for the end of the animation, after which we change it to default
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            _animator.Play("Idle");
            _countdown = new TimeSpan(0, 0, 5);
            EventState++;
        }
    }

    /// <summary>
    /// We check who survived and give him the opportunity to shoot
    /// </summary>
    protected void UpdateFinishingState(ref string text, ref string killerText, ref string targetText)
    {
        text = Translation.Cycle;
        killerText = Translation.Defeat;
        targetText = Translation.Lose;

        if (_countdown.TotalSeconds > 0)
            return;

        // Give another player the ability to use a shotgun
        if (_scientist.IsAlive)
        {
            Choser = _scientist;
            _classD = null;
        }
        else if (_classD.IsAlive)
        {
            Choser = _classD;
            _scientist = null;
        }
        else
        {
            Choser = null;
            _scientist = null;
            _classD = null;
        }

        EventState = 0;
        GunState = 0;
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
