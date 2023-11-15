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

namespace AutoEvent.Games.Gmod
{
    public class Plugin : Event, IEventMap, IInternalEvent, IHidden
    {
        public override string Name { get; set; } = "Sandbox";
        public override string Description { get; set; } = "A mode in which players can do anything.";
        public override string Author { get; set; } = "Logic - KoT0XleB; Map - GruumHD | pleechka | aria | KoT0XleB;";
        public override string CommandName { get; set; } = "gmod";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "gm_construct", Position = new Vector3(10f, 1015f, -50f), MapRotation = Quaternion.identity };
        private EventHandler EventHandler { get; set; }

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
            Players.Shot += EventHandler.OnPlayerShot;
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
            Players.Shot -= EventHandler.OnPlayerShot;
            EventHandler = null;
        }

        protected override void OnStart()
        {
            foreach (Player player in Player.GetPlayers())
            {
                //player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }
        }

        protected override bool IsRoundDone()
        {
            if (Player.GetPlayers().Count(r => r.IsAlive) > 0 && EventTime.TotalMinutes < 15) return false;
            else return true;
        }
        
        protected override void ProcessFrame()
        {
            var count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            Extensions.Broadcast("Gmod Cycle", 1);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.IsAlive) == 0)
            {
                Extensions.Broadcast("Mini-game Stopped", 10);
            }
            else
            {
                Extensions.Broadcast("Time is over", 10);
            }
        }
    }
}
