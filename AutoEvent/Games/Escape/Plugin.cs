using AutoEvent.Events.Handlers;
using CustomPlayerEffects;
using MapGeneration;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using Interactables.Interobjects;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Escape
{
    public class Plugin : Event, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = "Atomic Escape";
        public override string Description { get; set; } = "Escape from the facility behind SCP-173 at supersonic speed!";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "escape";
        public override Version Version { get; set; } = new Version(1, 0, 2);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public SoundInfo SoundInfo { get; set; } = new SoundInfo() 
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
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.CassieScp += _eventHandler.OnSendCassie;
            Players.PlaceTantrum += _eventHandler.OnPlaceTantrum;
        }
        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.CassieScp -= _eventHandler.OnSendCassie;
            Players.PlaceTantrum -= _eventHandler.OnPlaceTantrum;
            _eventHandler = null;
        }

        protected override bool IsRoundDone()
        {
            return !(EventTime.TotalSeconds <= Config.EscapeDurationTime && Player.GetPlayers().Count(r => r.IsAlive) > 0);
        }

        protected override void OnStart()
        {
            GameObject _startPos = new GameObject();
            _startPos.transform.parent = Facility.Rooms.First(r => r.Identifier.Name == RoomName.Lcz173).Transform;
            _startPos.transform.localPosition = new Vector3(16.5f, 13f, 8f);

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.Scp173, RoleSpawnFlags.None);
                player.Position = _startPos.transform.position;
                player.EffectsManager.EnableEffect<Ensnared>(10);
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
            foreach (Player player in Player.GetPlayers())
            {
                player.EffectsManager.EnableEffect<Flashed>(1);
                if (player.Position.y < 980f)
                {
                    player.Kill("You didn't have time");
                }
            }

            Extensions.Broadcast(Translation.End.Replace("{name}", Name).Replace("{players}", Player.GetPlayers().Count(x => x.IsAlive).ToString()), 10);
        }

        protected override void OnCleanup()
        {
            Warhead.IsLocked = false;
            Warhead.Stop();
        }
    }
}
