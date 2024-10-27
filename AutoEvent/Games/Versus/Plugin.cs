using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using MEC;
using PlayerRoles;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;
using System.Text;

namespace AutoEvent.Games.Versus
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent, IEventTag
    {
        public override string Name { get; set; } = "Cock Fights";
        public override string Description { get; set; } = "Duel of players on the 35hp map from cs 1.6";
        public override string Author { get; set; } = "RisottoMan/code & xleb.ik/map";
        public override string CommandName { get; set; } = "versus";
        public override Version Version { get; set; } = new Version(1, 0, 3);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "35Hp",
            Position = new Vector3(0, 30, 30)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Knife.ogg",
            Volume = 10
        };
        public TagInfo TagInfo { get; set; } = new()
        {
            Name = "Halloween",
            Color = "#ff0000"
        };
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        internal Player Scientist;
        internal Player ClassD;
        private EventHandler _eventHandler;
        private List<GameObject> _triggers;
        private List<GameObject> _teleports;
        private TimeSpan _countdown;
        private EventState _eventState;

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
            Players.ChargingJailbird += _eventHandler.OnJailbirdCharge;
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
            Players.ChargingJailbird -= _eventHandler.OnJailbirdCharge;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            Scientist = null;
            ClassD = null;
            _eventState = 0;
            _triggers = new();
            _teleports = new();
            _countdown = new TimeSpan(0, 0, Config.AutoSelectDelayInSeconds);

            if (Config.Team1Loadouts == Config.Team2Loadouts)
            {
                DebugLogger.LogDebug("Warning: Teams should not have the same roles.", LogLevel.Warn, true);
            }

            List<GameObject> spawnpoints = new();
            foreach (GameObject block in MapInfo.Map.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "Trigger": _triggers.Add(block); break;
                    case "Teleport": _teleports.Add(block); break;
                    case "Spawnpoint": spawnpoints.Add(block); break;
                }
            }

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)     
                {              
                    player.GiveLoadout(Config.Team1Loadouts);
                    player.Position = spawnpoints.ElementAt(0).transform.position;
                }
                else
                {
                    player.GiveLoadout(Config.Team2Loadouts);
                    player.Position = spawnpoints.ElementAt(1).transform.position;
                }
                count++;

                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.2f, () => { player.CurrentItem = item; });
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

        protected override bool IsRoundDone()
        {
            _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
            // At least 1 player on scientists && At least 1 player on dbois
            return !(Player.GetPlayers().Any(ply => Config.Team1Loadouts.Any(loadout => loadout.Roles.Any(role => ply.Role == role.Key))) &&
                     Player.GetPlayers().Any(ply => Config.Team2Loadouts.Any(loadout => loadout.Roles.Any(role => ply.Role == role.Key))));
        }

        protected override void ProcessFrame()
        {
            switch (_eventState)
            {
                case EventState.Waiting: UpdateWaitingState(); break;
                case EventState.ChooseScientist: Scientist = UpdateChoosePlayerState(true); break;
                case EventState.ChooseClassD: ClassD = UpdateChoosePlayerState(false); break;
                case EventState.Playing: UpdatePlayingState(); break;
            }

            string text = string.Empty;
            if (ClassD is null && Scientist is null)
            {
                text = Translation.PlayersNull;
            }
            else if (ClassD is null)
            {
                text = Translation.ClassDNull.Replace("{scientist}", Scientist.Nickname);
            }
            else if (Scientist is null)
            {
                text = Translation.ScientistNull.Replace("{classd}", ClassD.Nickname);
            }
            else
            {
                text = Translation.PlayersDuel.Replace("{scientist}", Scientist.Nickname).
                    Replace("{classd}", ClassD.Nickname);
            }

            Extensions.Broadcast(text.Replace("{name}", Name).Replace("{remain}", $"{_countdown.TotalSeconds}"), 1);
        }

        /// <summary>
        /// Updating variables before starting the game
        /// </summary>
        protected void UpdateWaitingState()
        {
            _countdown = new TimeSpan(0, 0, Config.AutoSelectDelayInSeconds);

            if (Scientist is null)
            {
                if (ClassD is not null)
                    ClassD.Heal(100);

                _eventState = EventState.ChooseScientist;
                return;
            }

            if (ClassD is null)
            {
                if (Scientist is not null)
                    Scientist.Heal(100);

                _eventState = EventState.ChooseClassD;
                return;
            }

            _eventState = EventState.Playing;
        }

        /// <summary>
        /// Choosing a new player
        /// </summary>
        protected Player UpdateChoosePlayerState(bool isScientist)
        {
            ushort value = 0;
            RoleTypeId role = RoleTypeId.Scientist;
            Player chosenPlayer;

            if (isScientist is not true)
            {
                value = 1;
                role = RoleTypeId.ClassD;
            }

            foreach (Player player in Player.GetPlayers())
            {
                if (player.Role != role)
                    continue;

                if (Vector3.Distance(player.Position, _triggers.ElementAt(value).transform.position) <= 1f)
                {
                    chosenPlayer = player;
                    goto End;
                }
            }

            if (_countdown.TotalSeconds > 0)
                return null;

            chosenPlayer = Player.GetPlayers().Where(r => r.Role == role).ToList().RandomItem();
            goto End;

        End:
            chosenPlayer.Position = _teleports.ElementAt(value).transform.position;
            _eventState = EventState.Waiting;
            return chosenPlayer;
        }

        /// <summary>
        /// Game in process
        /// </summary>
        protected void UpdatePlayingState()
        {
            if (ClassD is null || Scientist is null)
                _eventState = 0;
        }

        protected override void OnFinished()
        {
            string text = string.Empty;

            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) == 0)
            {
                text = Translation.ClassDWin.Replace("{name}", Name);
            }
            else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                text = Translation.ScientistWin.Replace("{name}", Name);
            }

            Extensions.Broadcast(text, 10);
        }
    }
}
