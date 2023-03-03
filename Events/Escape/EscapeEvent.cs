using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Events
{
    internal class EscapeEvent : IEvent
    {
        public string Name => AutoEvent.Singleton.Translation.EscapeName;
        public string Description => AutoEvent.Singleton.Translation.EscapeDescription;
        public string Color => "FFFF00";
        public string CommandName => "escape";
        public static TimeSpan EventTime { get; set; }

        public void OnStart()
        {
            Exiled.Events.Handlers.Player.Verified += EscapeHandler.OnJoin;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage += EscapeHandler.OnSendCassie;
            Exiled.Events.Handlers.Server.RespawningTeam += EscapeHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= EscapeHandler.OnJoin;
            Exiled.Events.Handlers.Cassie.SendingCassieMessage -= EscapeHandler.OnSendCassie;
            Exiled.Events.Handlers.Server.RespawningTeam -= EscapeHandler.OnTeamRespawn;
            Timing.CallDelayed(5f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Player.List.ToList().ForEach(player =>
            {
                player.Role.Set(RoleTypeId.Scp173, SpawnReason.None, RoleSpawnFlags.All);
                player.EnableEffect(EffectType.Ensnared, 10);
            });
            // we need Running in the 90's and Vicky Vale - Dancing lmao :D
            Extensions.PlayAudio("Escape.ogg", 25, true, Name);
            // Warhead started
            Warhead.DetonationTimer = 120f;
            Warhead.Start();
            Warhead.IsLocked = true;

            Timing.RunCoroutine(OnEventRunning(), "escape_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;
            // Countdown before the start of the game
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(trans.EscapeBeforeStart.Replace("{name}", Name).Replace("{time}", ((int)time).ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
            var explosionTime = 80;
            // Counting down
            while (EventTime.TotalSeconds != explosionTime && Player.List.Count(r => r.IsAlive) > 0)
            {
                Extensions.Broadcast(trans.EscapeCycle.Replace("{name}", Name).Replace("{time}", (explosionTime - EventTime.TotalSeconds).ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
            // Disable Warhead
            Warhead.IsLocked = false;
            Warhead.Stop();
            // We pretend that the warhead exploded so that we can conduct this mini-game many times.
            foreach (Player player in Player.List)
            {
                player.EnableEffect<CustomPlayerEffects.Flashed>(1);
                if (player.Position.y < 980f)
                {
                    player.Kill(DamageType.Warhead);
                }
            }
            Extensions.Broadcast(trans.EscapeEnd.Replace("{name}", Name), 10);
            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
        }
    }
}
