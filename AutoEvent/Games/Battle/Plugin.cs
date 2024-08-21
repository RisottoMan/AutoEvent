using MER.Lite.Objects;
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
        public override string Name { get; set; } = "Battle";
        public override string Description { get; set; } = "MTF fight against CI in an arena";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "battle";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "Battle", 
            Position = new Vector3(6f, 1030f, -43.5f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "MetalGearSolid.ogg", 
            Volume = 10, 
            Loop = false 
        };
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        private EventHandler _eventHandler { get; set; }
        private List<GameObject> _workstations;
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
            _eventHandler = null;
        }

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

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 20; time > 0; time--)
            {
                Extensions.Broadcast(Translation.TimeLeft.Replace("{time}", $"{time}"), 5);
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
            var text = Translation.Counter;
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
                Extensions.Broadcast(Translation.CiWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 3);
            }
            else // if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.MtfWin.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
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
