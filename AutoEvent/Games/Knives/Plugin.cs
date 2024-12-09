using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Knives
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Knives of Death";
        public override string Description { get; set; } = "Knife players against each other on a 35hp map from cs 1.6";
        public override string Author { get; set; } = "RisottoMan/code & xleb.ik/map";
        public override string CommandName { get; set; } = "knives";
        public override Version Version { get; set; } = new Version(1, 0, 3);
        [EventConfig]
        public KnivesConfig Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "35hp_2", 
            Position = new Vector3(5f, 1030f, -45f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Knife.ogg", 
            Volume = 10
        };
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        private EventHandler _eventHandler { get; set; }
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler();
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
            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    player.GiveLoadout(Config.Team1Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    player.GiveLoadout(Config.Team2Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }
                count++;

                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = item; });
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }   
        }

        protected override void CountdownFinished()
        {
            foreach(var wall in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Wall"))
            {
                GameObject.Destroy(wall);
            }
        }

        protected override bool IsRoundDone()
        {   
            return !(Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > 0 &&
                   Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) > 0);
        }

        protected override void ProcessFrame()
        {
            string mtfCount = Player.GetPlayers().Count(r => r.Team == Team.FoundationForces).ToString();
            string chaosCount = Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency).ToString();
            Extensions.Broadcast(Translation.Cycle.
                Replace("{name}", Name).
                Replace("{mtfcount}", mtfCount).
                Replace("{chaoscount}", chaosCount), 1);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.ChaosWin.Replace("{name}", Name), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.MtfWin.Replace("{name}", Name), 10);
            }
        }
    }
}
