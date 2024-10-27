using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using InventorySystem.Items.MarshmallowMan;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Infection
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent, IEventTag
    {
        public override string Name { get; set; } = "Zombie Infection";
        public override string Description { get; set; } = "Zombie mode, the purpose of which is to infect all players";
        public override string Author { get; set; } = "RisottoMan";
        public override string CommandName { get; set; } = "zombie";
        public override Version Version { get; set; } = new Version(1, 0, 4);
        [EventConfig] 
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "Zombie", 
            Position = new Vector3(115.5f, 1030f, -43.5f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Zombie_Run.ogg", 
            Volume = 15
        };
        public TagInfo TagInfo { get; set; } = new()
        {
            Name = "Halloween",
            Color = "#ff0000"
        };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler _eventHandler { get; set; }
        private int _overtime = 30;
        public bool IsChristmasUpdate { get; set; } = false;
        public bool IsHalloweenUpdate { get; set; } = false;

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
            _overtime = 30;
            // Halloween update
            if (Enum.IsDefined(typeof(ItemType), "Marshmallow"))
            {
                IsHalloweenUpdate = true;
                ForceEnableFriendlyFire = FriendlyFireSettings.Enable;
            }
            
            foreach (Player player in Player.GetPlayers())
            {
                /* // Christmas Update
                if (IsFlamingoVariant == true)
                {
                    player.GiveLoadout(Config.FlamingoLoadouts);
                }
                */
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = RandomPosition.GetSpawnPosition(MapInfo.Map);
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.Start.
                    Replace("{name}", Name).
                    Replace("{time}", time.ToString("00")), 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            Player player = Player.GetPlayers().RandomItem();
            /* // ChristmasUpdate
            if (IsFlamingoVariant == true)
            {
                player.GiveLoadout(Config.ZombieFlamingoLoadouts);
            }
            */
            
            if (IsHalloweenUpdate)
            {
                player.SetRole(RoleTypeId.Scientist, RoleSpawnFlags.None);
                player.EffectsManager.EnableEffect<MarshmallowEffect>();
                player.IsGodModeEnabled = true;
            }
            else
            {
                player.GiveLoadout(Config.ZombieLoadouts);
            }
            
            Extensions.PlayPlayerAudio(player, Config.ZombieScreams.RandomItem(), 15);
        }

        protected override bool IsRoundDone()
        {
            /* // Christmas Update
            if (IsFlamingoVariant == true)
            {
                if (Player.GetPlayers().Count(r => 
                r.Role == RoleTypeId.Flamingo || 
                r.Role == RoleTypeId.AlphaFlamingo) > 0
                && _overtime > 0) return false;
                else return true;
            }
            */

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) > 0 && _overtime > 0) 
                return false;
            return true;
        }
        
        protected override void ProcessFrame()
        {
            int count = 0;
            string time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            
            /* // Christmas Update
            if (IsFlamingoVariant == true)
            {
                count = Player.GetPlayers().Count(r =>
                r.Role == RoleTypeId.Flamingo ||
                r.Role == RoleTypeId.AlphaFlamingo);
            }
            */
            count = Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD);

            if (count > 1)
            {
                Extensions.Broadcast(Translation.Cycle.
                    Replace("{name}", Name).
                    Replace("{count}", count.ToString()).
                    Replace("{time}", time), 1);
            }
            else if (count == 1)
            {
                _overtime--;
                Extensions.Broadcast(Translation.ExtraTime
                        .Replace("{extratime}", _overtime.ToString("00"))
                        .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 1);
            }
        }

        protected override void OnFinished()
        {
            /* // Christmas Update
            if (IsFlamingoVariant == true)
            {
                if (Player.GetPlayers().Count(r => 
                r.Role == RoleTypeId.Flamingo ||
                r.Role == RoleTypeId.AlphaFlamingo) == 0)
                {
                    Extensions.Broadcast(Translation.ZombieWin
                        .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
                }
                else
                {
                    Extensions.Broadcast(Translation.ZombieLose
                        .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
                }
            }*/
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(Translation.Win
                    .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.Lose
                    .Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }
        }
    }
}
