using MER.Lite.Objects;
using AutoEvent.Events.Handlers;
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

namespace AutoEvent.Games.FinishWay
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.FinishWayTranslate.FinishWayName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.FinishWayTranslate.FinishWayDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.FinishWayTranslate.FinishWayCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public FinishWayConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "FinishWay", Position = new Vector3(115.5f, 1030f, -43.5f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "FinishWay.ogg", Volume = 8, Loop = false, StartAutomatically = false };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private FinishWayTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.FinishWayTranslate;
        private TimeSpan _remainingTime;
        private GameObject _point;

        protected override void RegisterEvents()
        { EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= EventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= EventHandler.OnPlaceBlood;
            Players.DropItem -= EventHandler.OnDropItem;
            Players.DropAmmo -= EventHandler.OnDropAmmo;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            foreach (Player player in Player.GetPlayers())
            {
                //Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.GiveLoadout(Config.Loadouts);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }

        }


        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<b>{time}</b>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }
        protected override void CountdownFinished()
        {
            _remainingTime = new TimeSpan(0, 0, Config.EventDurationInSeconds);
            StartAudio();
            _point = new GameObject();
            foreach(var gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Wall": { GameObject.Destroy(gameObject); } break;
                    case "Lava": { gameObject.AddComponent<LavaComponent>(); } break;
                    case "FinishTrigger": { _point = gameObject; } break;
                }
            }
        }

        protected override bool IsRoundDone()
        {
            // At least one player is alive &&
            // Elapsed time is shorter than a minute (+ broadcast duration)
            return !(Player.GetPlayers().Count(r => r.IsAlive) > 0 && EventTime.TotalSeconds < Config.EventDurationInSeconds );
        }

        protected override void ProcessFrame()
        {

            var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
            var time = $"{_remainingTime.Minutes:00}:{_remainingTime.Seconds:00}";

            Extensions.Broadcast(Translation.FinishWayCycle.Replace("{name}", Name).Replace("{time}", time), 1);
            _remainingTime -= TimeSpan.FromSeconds(FrameDelayInSeconds);
        }

        protected override void OnFinished()
        {
            foreach(Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _point.transform.position) > 10)
                {
                    player.Kill(Translation.FinishWayDied);
                }
            }

            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(Translation.FinishWaySeveralSurvivors.Replace("{count}", Player.GetPlayers().Count(r => r.IsAlive).ToString()), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.FinishWayOneSurvived.Replace("{player}", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.FinishWayNoSurvivors, 10);
            }
        }
    }
}
