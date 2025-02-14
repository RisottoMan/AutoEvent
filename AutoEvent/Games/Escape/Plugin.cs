using CustomPlayerEffects;
using MapGeneration;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using Exiled.API.Enums;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using Interactables.Interobjects;
using Player = Exiled.API.Features.Player;
using Warhead = Exiled.API.Features.Warhead;

namespace AutoEvent.Games.Escape;
public class Plugin : Event<Config, Translation>, IEventSound
{
    public override string Name { get; set; } = "Atomic Escape";
    public override string Description { get; set; } = "Escape from the facility behind SCP-173 at supersonic speed!";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "escape";
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "Escape.ogg", 
        Volume = 25, 
        Loop = false
    };
    private EventHandler _eventHandler { get; set; }
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Joined += _eventHandler.OnJoined;
        Exiled.Events.Handlers.Map.AnnouncingScpTermination += _eventHandler.OnAnnoucingScpTermination;
        Exiled.Events.Handlers.Scp173.PlacingTantrum += _eventHandler.OnPlacingTantrum;
        Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds += _eventHandler.OnUsingBreakneckSpeeds;
    }
    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Joined -= _eventHandler.OnJoined;
        Exiled.Events.Handlers.Map.AnnouncingScpTermination -= _eventHandler.OnAnnoucingScpTermination;
        Exiled.Events.Handlers.Scp173.PlacingTantrum -= _eventHandler.OnPlacingTantrum;
        Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds -= _eventHandler.OnUsingBreakneckSpeeds;
        _eventHandler = null;
    }

    protected override bool IsRoundDone()
    {
        return !(EventTime.TotalSeconds <= Config.EscapeDurationTime && Player.List.Count(r => r.IsAlive) > 0);
    }

    protected override void OnStart()
    {
        GameObject _startPos = new GameObject();
        _startPos.transform.parent = Room.List.First(r => r.Identifier.Name == RoomName.Lcz173).Transform;
        _startPos.transform.localPosition = new Vector3(16.5f, 13f, 8f);

        foreach (Player player in Player.List)
        {
            player.GiveLoadout(Config.Scp173Loadout);
            player.Position = _startPos.transform.position;
            player.EnableEffect(EffectType.Ensnared, 1, 10);
            player.EnableEffect(EffectType.MovementBoost, 50);
        }

        AlphaWarheadController.Singleton.CurScenario.AdditionalTime = Config.EscapeResumeTime;
        Warhead.Start();
        Warhead.IsLocked = true;
    }

    protected override void ProcessFrame()
    {
        Extensions.Broadcast(Translation.Cycle.Replace("{name}", Name).Replace("{time}", (Config.EscapeDurationTime - EventTime.TotalSeconds).ToString("00")), 1);
    }

    protected override IEnumerator<float> BroadcastStartCountdown()
    {
        for (int time = 10; time > 0; time--)
        {
            Extensions.Broadcast(
                Translation.BeforeStart.Replace("{name}", Name).Replace("{time}", ((int)time).ToString()), 1);
            yield return Timing.WaitForSeconds(1f);
        }

        foreach (DoorVariant door in DoorVariant.AllDoors)
        {
            if (door is not ElevatorDoor)
            {
                door.NetworkTargetState = true;
            }
        }
        
        yield break;
    }

    protected override void OnFinished()
    {
        foreach (Player player in Player.List)
        {
            player.EnableEffect<Flashed>(1, 1);
            
            if (player.Position.y < 980f)
            {
                player.Kill("You didn't have time");
            }
        }
        
        string playeAlive = Player.List.Count(x => x.IsAlive).ToString();
        Extensions.Broadcast(Translation.End.Replace("{name}", Name).Replace("{players}", playeAlive), 10);
    }

    protected override void OnCleanup()
    {
        Warhead.IsLocked = false;
        Warhead.Stop();
    }
}