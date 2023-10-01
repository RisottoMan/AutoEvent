using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Battle
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        // Set the info for the event.
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.BattleTranslate.BattleName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.BattleTranslate.BattleDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } =  AutoEvent.Singleton.Translation.BattleTranslate.BattleCommandName;

        // Map Info can be inherited as long as the event inherits IEventMap.
        // MapInfo.Map is the Schematic Object for the map.
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "Battle", Position = new Vector3(6f, 1030f, -43.5f) };

        // Sound Info can be inherited as long as the event inherits IEventSound.
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "MetalGearSolid.ogg", Volume = 10, Loop = false };
        
        // Define the fields/properties here. Make sure to set them, in OnStart() or OnRegisteringEvents()
        // Define the properties that may be used by this event, or by its handler class.
        private EventHandler EventHandler { get; set; }
        private BattleTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.BattleTranslate;
        
        [EventConfig]
        public BattleConfig Config { get; set; }
        
        // Define the fields that will only be used inside this event class.
        private List<GameObject> _workstations;
        
        // Events only need to be registered when the GameMode Event is being run.
        protected override void RegisterEvents()
        {
            EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

        // Events are unregistered when the GameMode Event is finished.
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

        // Define what you want to happen when the even is started / run.
        protected override void OnStart()
        {
            int count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    Extensions.SetRole(player, RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }

                count++;

                player.GiveLoadout(Config.Loadouts, LoadoutFlags.IgnoreRole);
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = player.Items.FirstOrDefault(x => x); });
            }

        }

        // Override this coroutine if you want a start countdown. The coroutine runs automatically
        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            // Count down the time until start. we use a 20 second timer for this.
            for (int time = 20; time > 0; time--)
            {
                Extensions.Broadcast(Translation.BattleTimeLeft.Replace("{time}", $"{time}"), 5);
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }

        protected override void CountdownFinished()
        {
            // Once the countdown has ended, we need to destroy the walls, and add workstations.
            _workstations = new List<GameObject>();
            foreach (var gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Wall": { GameObject.Destroy(gameObject); } break;
                    case "Workstation": { _workstations.Add(gameObject); } break;
                }
            }
        }


        protected override bool IsRoundDone()
        {
            // Round finishes when either team has no more players.
            return !EndConditions.TeamHasMoreThanXPlayers(Team.FoundationForces,0) ||
                   !EndConditions.TeamHasMoreThanXPlayers(Team.ChaosInsurgency,0);
        }

        // Default is 1f
        protected override float FrameDelayInSeconds { get; set; } = 1f;

        protected override void ProcessFrame()
        {
            // While the round isn't done, this will be called once a second. You can make the call duration faster / slower by changing FrameDelayInSeconds.
            // While the round is still going, broadcast the current round stats.
            var text = Translation.BattleCounter;
            text = text.Replace("{FoundationForces}", $"{Player.GetPlayers().Count(r => r.Team == Team.FoundationForces)}");
            text = text.Replace("{ChaosForces}", $"{Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency)}");
            text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            // Once the round is finished, broadcast the winning team (either mtf or chaos in this case.)
            // If the round is stopped, this wont be called. Instead use OnStop to broadcast either winners, or that nobody wins because the round was stopped.
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.BattleCiWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 3);
            }
            else // if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.BattleMtfWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }        
        }

        protected override float PostRoundDelay { get; set; } = 10f;

        protected override void OnCleanup()
        {
            // 10 seconds after finishing the round or once the round is stopped, this will be called.
            // If 10 seconds is too long, you can change PostRoundDelay to make it faster or shorter.
            // We can cleanup extra workstations that we spawned in. 
            // The map will be cleaned up for us, as well as items, ragdolls, and sound.
            foreach (var bench in _workstations)
                GameObject.Destroy(bench);
        }
        
        
    }
}
