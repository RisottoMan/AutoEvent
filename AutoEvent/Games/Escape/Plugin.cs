using CustomPlayerEffects;
using MapGeneration;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using Interactables.Interobjects;
using PluginAPI.Core;
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
    protected override float PostRoundDelay { get; set; } = 5f;
    private EventHandler _eventHandler { get; set; }
    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler();
        Exiled.Events.Handlers.Player.Joined += _eventHandler.OnJoined;
        Exiled.Events.Handlers.Map.AnnouncingScpTermination += _eventHandler.OnAnnoucingScpTermination;
        Exiled.Events.Handlers.Scp173.PlacingTantrum += _eventHandler.OnPlacingTantrum;
    }
    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Joined -= _eventHandler.OnJoined;
        Exiled.Events.Handlers.Map.AnnouncingScpTermination -= _eventHandler.OnAnnoucingScpTermination;
        Exiled.Events.Handlers.Scp173.PlacingTantrum -= _eventHandler.OnPlacingTantrum;
        _eventHandler = null;
    }

    protected override bool IsRoundDone()
    {
        return !(EventTime.TotalSeconds <= Config.EscapeDurationTime && Player.List.Count(r => r.IsAlive) > 0);
    }

    protected override void OnStart()
    {
        GameObject _startPos = new GameObject();
        _startPos.transform.parent = Facility.Rooms.First(r => r.Identifier.Name == RoomName.Lcz173).Transform;
        _startPos.transform.localPosition = new Vector3(16.5f, 13f, 8f);

        foreach (Player player in Player.List)
        {
            player.Role.Set(RoleTypeId.Scp173, RoleSpawnFlags.None);
            player.Position = _startPos.transform.position;
            player.EnableEffect<Ensnared>(10);
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
            player.EnableEffect<Flashed>(1);
            if (player.Position.y < 980f)
            {
                player.Kill("You didn't have time");
            }
        }

        Extensions.Broadcast(Translation.End.Replace("{name}", Name).Replace("{players}", Player.List.Count(x => x.IsAlive).ToString()), 10);
    }

    protected override void OnCleanup()
    {
        Warhead.IsLocked = false;
        Warhead.Stop();
    }
}