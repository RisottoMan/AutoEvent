using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.AmongUs
{
    public class Plugin : Event, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "Among Us";
        public override string Description { get; set; } = "You have to survive on a spaceship against the importers";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "amongus";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "AmongUs",
            Position = new Vector3(0, 30, 30)
        };
        private EventHandler _eventHandler;
        private EventState _eventState;
        private List<GameObject> _spawnpoints;
        private TimeSpan _countdown;
        private GameObject _shipObject;
        internal List<Player> ImposterList;
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
            _eventState = 0;
            _spawnpoints = new();
            _countdown = new TimeSpan(0, 0, 30);

            foreach (GameObject block in MapInfo.Map.AttachedBlocks)
            {
                switch (block.name)
                {
                    case "Spawnpoint": _spawnpoints.Add(block); break;
                    case "Ship": _shipObject = block; break;
                    //case "Teleport": _teleports.Add(block); break;
                }
            }

            foreach (Player player in Player.GetPlayers())
            {
                //player.GiveLoadout(Config.Team1Loadouts);
                player.Position = _shipObject.transform.position + Vector3.one * 2;
            }
        }

        protected override bool IsRoundDone()
        {
            _countdown = _countdown.TotalSeconds > 0 ? _countdown.Subtract(new TimeSpan(0, 0, 1)) : TimeSpan.Zero;
            return !(Player.GetPlayers().Where(r => r.IsAlive).Count() > 0 && _eventState != EventState.Ending);
        }

        protected override void ProcessFrame()
        {
            string text = string.Empty;
            switch (_eventState)
            {
                case EventState.Waiting: UpdateWaitingState(ref text); break;
                case EventState.Starting: UpdateStartingState(ref text); break;
                case EventState.Playing: UpdatePlayingState(ref text); break;
                case EventState.Calling: UpdateCallingState(ref text); break;
                case EventState.Warning: UpdateWarningState(ref text); break;
                case EventState.Ending: UpdateEndingState(ref text); break;
            }

            Extensions.Broadcast(text, 1);
        }

        /// <summary>
        /// We are waiting for all the players until they press the button
        /// </summary>
        protected void UpdateWaitingState(ref string text)
        {
            // Когда отсчет закончится, телепортируем игроков на локацию
            if (_countdown.TotalSeconds > 0)
                return;

            foreach (Player player in Player.GetPlayers())
            {
                player.Position = _spawnpoints.RandomItem().transform.position;
            }

            _eventState = EventState.Starting;
        }

        /// <summary>
        /// Starting the game
        /// </summary>
        protected void UpdateStartingState(ref string text)
        {
            // Сколько необходимо трейтеров в игре?
            int impostorCount = 0;
            switch (Player.GetPlayers().Count())
            {
                case int n when (n > 0 && n <= 10): impostorCount = 2; break;
                case int n when (n > 10 && n <= 20): impostorCount = 4; break;
                case int n when (n > 30): impostorCount = 6; break;
            }

            // Выбираем трейтеров исходя из количества игроков
            List<Player> crewmateList = Player.GetPlayers();
            ImposterList = new();
            for (int i = 0; i < impostorCount; i++)
            {
                Player ply = crewmateList.RandomItem();
                ImposterList.Add(ply);
                crewmateList.Remove(ply);
            }

            // Делаем выбранных игроков трейтерами
            foreach(Player impostor in ImposterList)
            {
                // Что-то тут делаем
            }

            _eventState = EventState.Playing;
        }

        /// <summary>
        /// Game in process
        /// </summary>
        protected void UpdatePlayingState(ref string text)
        {

        }

        /// <summary>
        /// Someone pressed a button or found a corpse
        /// </summary>
        protected void UpdateCallingState(ref string text)
        {

        }

        /// <summary>
        /// An event in which a task must be completed quickly
        /// </summary>
        protected void UpdateWarningState(ref string text)
        {

        }

        /// <summary>
        /// The end of the mini game
        /// </summary>
        protected void UpdateEndingState(ref string text)
        {

        }

        protected override void OnFinished()
        {

        }
    }
}
