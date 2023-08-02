using MapGeneration;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Events.Escape
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.EscapeName;
        public override string Description { get; set; } = Translation.EscapeDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; }
        public override string CommandName { get; set; } = "escape";
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            OnEventStarted();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
            _eventHandler = null;
            Timing.CallDelayed(5f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            GameObject _startPos = new GameObject();
            _startPos.transform.parent = Facility.Rooms.First(r => r.Identifier.Name == RoomName.Lcz173).Transform;
            _startPos.transform.localPosition = new Vector3(16.5f, 13f, 8f);

            foreach(Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.Scp173, RoleChangeReason.None);
                player.Position = _startPos.transform.position;
                player.EffectsManager.EnableEffect<CustomPlayerEffects.Ensnared>(10);
            }

            Extensions.PlayAudio("Escape.ogg", 25, true, Name);

            Warhead.DetonationTime = 120f;
            Warhead.Start();
            Warhead.IsLocked = true;

            Timing.RunCoroutine(OnEventRunning(), "escape_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(Translation.EscapeBeforeStart.Replace("{name}", Name).Replace("{time}", ((int)time).ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
            var explosionTime = 80;

            while (EventTime.TotalSeconds != explosionTime && Player.GetPlayers().Count(r => r.IsAlive) > 0)
            {
                Extensions.Broadcast(Translation.EscapeCycle.Replace("{name}", Name).Replace("{time}", (explosionTime - EventTime.TotalSeconds).ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            Warhead.IsLocked = false;
            Warhead.Stop();

            foreach (Player player in Player.GetPlayers())
            {
                player.EffectsManager.EnableEffect<CustomPlayerEffects.Flashed>(1);
                if (player.Position.y < 980f)
                {
                    player.Kill("You didn't have time");
                }
            }

            Extensions.Broadcast(Translation.EscapeEnd.Replace("{name}", Name), 10);

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
