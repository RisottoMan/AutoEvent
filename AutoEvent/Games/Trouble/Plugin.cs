using System;
using AutoEvent.Events.Handlers;
using PluginAPI.Events;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Trouble
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent, IHidden
    {
        // Removed Mini-Game for Haloween Update
        public override string Name { get; set; } = "Trouble in Terrorist Town";
        public override string Description { get; set; } = "An impostor appeared in terrorist town";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "trouble";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "AmongUs", // Deleted
            Position = new Vector3(115.5f, 1030f, -43.5f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Skeleton.ogg", 
            Volume = 5,
        };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler _eventHandler { get; set; }

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PlayerDamage -= _eventHandler.OnPlayerDamage;
            _eventHandler = null;
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
            string time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            string text = string.Empty;

            if (Player.GetPlayers().Count(r => r.IsHuman) > Player.GetPlayers().Count(r => r.IsSCP))
            {
                text = Translation.TroubleHumanWin.Replace("{time}", time);
            }
            else if (Player.GetPlayers().Count(r => r.IsHuman) < Player.GetPlayers().Count(r => r.IsSCP))
            {
                text = Translation.TroubleTraitorWin.Replace("{time}", time);
            }
            else
            {
                text = Translation.TroubleEveryoneDied.Replace("{time}", time);
            }

            Extensions.Broadcast(text, 10);
        }
    }
}