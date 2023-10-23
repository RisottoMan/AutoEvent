using MER.Lite.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Infection
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.InfectTranslate.ZombieName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.InfectTranslate.ZombieDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.InfectTranslate.ZombieCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig] public InfectConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "Zombie", Position = new Vector3(115.5f, 1030f, -43.5f), MapRotation = Quaternion.identity };

        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Zombie.ogg", Volume = 7, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private InfectTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.InfectTranslate;
        private int _overtime = 30;

        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler(this);
            EventManager.RegisterEvents(EventHandler);

            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
            Players.PlayerDamage += EventHandler.OnPlayerDamage;
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
            Players.PlayerDamage -= EventHandler.OnPlayerDamage;
            EventHandler = null;
        }

        protected override void OnStart()
        {
            _overtime = 30;

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }

            float scale = 1;
            switch(Player.GetPlayers().Count())
            {
                case var n when (n > 15 && n <= 20): scale = 1.1f; break;
                case var n when (n > 20 && n <= 25): scale = 1.2f; break;
                case var n when (n > 25 && n <= 30): scale = 1.3f; break;
                case var n when (n > 30 && n <= 35): scale = 1.4f; break;
                case var n when (n > 35): scale = 1.5f; break;
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.ZombieBeforeStart.Replace("{name}", Name).Replace("{time}", time.ToString("00")), 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            Player.GetPlayers().RandomItem().GiveLoadout(Config.ZombieLoadouts);
        }

        protected override bool IsRoundDone()
        {
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0 && _overtime > 0) return false;
            else return true;
        }
        
        protected override void ProcessFrame()
        {
            var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";

            if (count > 1)
            {
                Extensions.Broadcast(Translation.ZombieCycle.Replace("{name}", Name).Replace("{count}", count.ToString()).Replace("{time}", time), 1);
            }
            else if (count == 1)
            {
                _overtime--;
                Extensions.Broadcast(
                    Translation.ZombieExtraTime
                        .Replace("{extratime}", _overtime.ToString("00"))
                        .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 1);
            }
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(Translation.ZombieWin
                    .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.ZombieLose
                    .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }
        }
    }
}
