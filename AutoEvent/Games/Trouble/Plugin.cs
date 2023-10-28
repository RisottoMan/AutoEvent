using AutoEvent.Events.Handlers;
using PluginAPI.Events;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Trouble
{
    public class Plugin : Event, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslate.Name;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslate.Description;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslate.CommandName;
        [EventConfig] public TroubleConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "AmongUs", Position = new Vector3(115.5f, 1030f, -43.5f), MapRotation = Quaternion.identity };

        //public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        //    { SoundName = "Zombie.ogg", Volume = 7, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private TroubleTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.TroubleTranslate;

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
            // random class - scientist / mtf / guard
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }

            Player traitor = Player.GetPlayers().RandomItem();
            traitor.GiveLoadout(Config.TraitorLoadouts);
            // change skin to human
        }

        protected override bool IsRoundDone()
        {
            if (Player.GetPlayers().Count(r => r.IsHuman) > Player.GetPlayers().Count(r => r.IsSCP) 
                && EventTime.TotalMinutes < 5) return false;
            else return true;
        }

        protected override void ProcessFrame()
        {
            // Trouble in Terrorist Town
            Extensions.Broadcast($"{Name}\n" +
                $"<color=red>{Player.GetPlayers().Count(r => r.IsSCP)} traitors</color> | " +
                $"<color=cyan>{Player.GetPlayers().Count(r => r.IsHuman)} guys</color>", 1);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.IsHuman) > Player.GetPlayers().Count(r => r.IsSCP))
            {
                Extensions.Broadcast($"{Name}\n" +
                    $"Humans winning", 1);
            }
            else if (Player.GetPlayers().Count(r => r.IsHuman) < Player.GetPlayers().Count(r => r.IsSCP))
            {
                Extensions.Broadcast($"{Name}\n" +
                    $"Traitors winning", 1);
            }
            else
            {
                Extensions.Broadcast($"{Name}\n" +
                    $"Draw | no one won", 1);
            }
        }
    }
}
