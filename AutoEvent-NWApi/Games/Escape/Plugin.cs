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
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Escape
{
    public class Plugin : Event, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.EscapeTranslate.EscapeName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.EscapeTranslate.EscapeDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "escape";
        [EventConfig]
        public EscapeConfig Config { get; set; }
        public SoundInfo SoundInfo { get; set; } =
            new SoundInfo() { SoundName = "Escape.ogg", Volume = 25, Loop = true};
        protected override float PostRoundDelay { get; set; } = 5f;
        private EventHandler EventHandler { get; set; }
        private EscapeTranslate _translation;

        protected override void RegisterEvents()
        {
            _translation = new EscapeTranslate();
            EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.CassieScp += EventHandler.OnSendCassie;
            Players.PlaceTantrum += EventHandler.OnPlaceTantrum;

        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.CassieScp -= EventHandler.OnSendCassie;
            Players.PlaceTantrum -= EventHandler.OnPlaceTantrum;

            EventHandler = null;
        }

        protected override bool IsRoundDone()
        {
            // Elapsed Time is smaller than the explosion time &&
            // At least one player is alive.
            return !(EventTime.TotalSeconds <= Config.EscapeDurationInSeconds + 10 && Player.GetPlayers().Count(r => r.IsAlive) > 0);
        }

        protected override void OnStart()
        {
            GameObject _startPos = new GameObject();
            _startPos.transform.parent = Facility.Rooms.First(r => r.Identifier.Name == RoomName.Lcz173).Transform;
            _startPos.transform.localPosition = new Vector3(16.5f, 13f, 8f);

            foreach(Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.Scp173, RoleSpawnFlags.None);
                player.Position = _startPos.transform.position;
                player.EffectsManager.EnableEffect<Ensnared>(10);
            }

            Warhead.DetonationTime = Config.EscapeDurationInSeconds + 20f;
            // Warhead.DetonationTime = 120f;
            Warhead.Start();
            Warhead.IsLocked = true;
        }

        protected override void ProcessFrame()
        {
            Extensions.Broadcast(_translation.EscapeCycle.Replace("{name}", Name).Replace("{time}", (Config.EscapeDurationInSeconds - EventTime.TotalSeconds).ToString("00")), 1);
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(
                    _translation.EscapeBeforeStart.Replace("{name}", Name).Replace("{time}", ((int)time).ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }

        protected override void OnFinished()
        {
            Warhead.IsLocked = false;
            Warhead.Stop();

            foreach (Player player in Player.GetPlayers())
            {
                player.EffectsManager.EnableEffect<Flashed>(1);
                if (player.Position.y < 980f)
                {
                    player.Kill("You didn't have time");
                }
            }

            Extensions.Broadcast(_translation.EscapeEnd.Replace("{name}", Name).Replace("{players}", Player.GetPlayers().Count(x => x.IsAlive).ToString()), 10);
        }

    }
}
