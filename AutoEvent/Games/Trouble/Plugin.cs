using System;
using AutoEvent.Events.Handlers;
using PluginAPI.Events;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;
using MEC;
using System.Collections.Generic;

namespace AutoEvent.Games.Trouble
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent, IHidden
    {
        // Haloween Update
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslation.TroubleName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslation.TroubleDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslation.TroubleCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public TroubleConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "AmongUs", 
            Position = new Vector3(115.5f, 1030f, -43.5f), 
            IsStatic = true
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { SoundName = "Skeleton.ogg", Volume = 5, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private TroubleTranslation Translation { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslation;

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
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.TroubleBeforeStart.Replace("{time}", time.ToString()), 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            for (int i = 0; i < (Player.GetPlayers().Count / 10) + 1; i++)
            {
                Player traitor = Player.GetPlayers().RandomItem();
                Extensions.SetRole(traitor, PlayerRoles.RoleTypeId.Scp3114, PlayerRoles.RoleSpawnFlags.AssignInventory);
            }
        }

        protected override bool IsRoundDone()
        {
            if (Player.GetPlayers().Count(r => r.IsHuman) >= Player.GetPlayers().Count(r => r.IsSCP) 
                && EventTime.TotalMinutes < 3) return false;
            else return true;
        }

        protected override void ProcessFrame()
        {
            var scpCount = Player.GetPlayers().Count(r => r.IsSCP);
            var humanCount = Player.GetPlayers().Count(r => r.IsHuman);
            var text = Translation.TroubleCycle
                .Replace("{name}", Name)
                .Replace("{scp}", scpCount.ToString())
                .Replace("{human}", humanCount.ToString());

            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            if (Player.GetPlayers().Count(r => r.IsHuman) > Player.GetPlayers().Count(r => r.IsSCP))
            {
                Extensions.Broadcast(Translation.TroubleHumanWin.Replace("{time}", time), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsHuman) < Player.GetPlayers().Count(r => r.IsSCP))
            {
                Extensions.Broadcast(Translation.TroubleTraitorWin.Replace("{time}", time), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.TroubleEveryoneDied.Replace("{time}", time), 10);
            }
        }
    }
}