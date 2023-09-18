using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using CustomPlayerEffects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.ZombieEscape
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } =
            AutoEvent.Singleton.Translation.ZombieEscapeTranslate.ZombieEscapeName;
        public override string Description { get; set; } =
            AutoEvent.Singleton.Translation.ZombieEscapeTranslate.ZombieEscapeDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "zombie3";
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "zm_osprey", Position = new Vector3(-15f, 1020f, -80f), Rotation = Quaternion.identity };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Survival.ogg", Volume = 10, Loop = false };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private ZombieEscapeTranslate Translation { get; set; }
        private SchematicObject _boat;
        private SchematicObject _heli;
        private GameObject _button;
        private GameObject _button1;
        private GameObject _button2;
        private GameObject _wall;
        private Vector3 _finishPosition;
        

        protected override void RegisterEvents()
        {
            Translation = new ZombieEscapeTranslate();

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
            if (EventHandler is null)
            {
                DebugLogger.LogDebug("Handler is null");
            }
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
        /*public Plugin(SchematicObject boat)
        {
            _boat = boat;
        }*/

        protected override void OnStart()
        {
            Server.FriendlyFire = false;
            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
                //Extensions.SetPlayerAhp(player, 100, 100, 0);

                var item = player.AddItem(RandomClass.GetRandomGun());
                player.AddItem(ItemType.GunCOM18);
                player.AddItem(ItemType.ArmorCombat);

                Timing.CallDelayed(0.1f, () => { player.CurrentItem = item; });
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float _time = 20; _time > 0; _time--)
            {
                Extensions.Broadcast(
                    Translation.ZombieEscapeBeforeStart.Replace("%name%", Name).Replace("%time%", $"{_time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            Extensions.PlayAudio("Zombie2.ogg", 7, false, Name);

            for (int i = 0; i <= Player.GetPlayers().Count() / 10; i++)
            {
                var playerList = Player.GetPlayers().Where(r => r.IsHuman);
                if (playerList.Count() > 0)
                {
                    var player = playerList.ToList().RandomItem();
                    DebugLogger.LogDebug($"{player.Nickname} chosen as the zombie.", LogLevel.Debug);
                    Extensions.SetRole(player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                    player.EffectsManager.EnableEffect<Disabled>();
                    player.EffectsManager.EnableEffect<Scp1853>();
                    player.Health = 10000;
                }
                else
                {
                    DebugLogger.LogDebug($"Not enough players to choose a random player. Skipping spawn.", LogLevel.Debug);
                }
            }

            _button = new GameObject();
            _button1 = new GameObject();
            _button2 = new GameObject();
            _wall = new GameObject();
            _finishPosition = new Vector3();
            
            foreach (var gameObject in MapInfo.Map.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Button":
                    {
                        _button = gameObject;
                    }
                        break;
                    case "Button1":
                    {
                        _button1 = gameObject;
                    }
                        break;
                    case "Button2":
                    {
                        _button2 = gameObject;
                    }
                        break;
                    case "Lava":
                    {
                        gameObject.AddComponent<LavaComponent>();
                    }
                        break;
                    case "Wall":
                    {
                        _wall = gameObject;
                    }
                        break;
                    case "Finish":
                    {
                        _finishPosition = gameObject.transform.position;
                    }
                        break;
                }
            }
            DebugLogger.LogDebug("Serialized GameObjects.", LogLevel.Debug);
        }

        protected override void ProcessFrame()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _button1.transform.position) < 3)
                {
                    _button1.transform.position += Vector3.down * 5;
                    _wall.AddComponent<WallComponent>();
                }

                if (Vector3.Distance(player.Position, _button2.transform.position) < 3)
                {
                    _button2.transform.position += Vector3.down * 5;
                    EventTime = new TimeSpan(0, 1, 5);
                    _heli = Extensions.LoadMap("Helicopter_Zombie", MapInfo.Map.Position, Quaternion.identity,
                        Vector3.one);
                }

                string text = Translation.ZombieEscapeHelicopter.Replace("%name%", Name)
                    .Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsHuman)}");
                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);
            }

        }

        protected override bool IsRoundDone()
        {
            // At least 1 human player alive &&
            // At least 1 scp player alive && 
            // Elapsed time is shorter than 5 minutes (+ countdown)
            return !(Player.GetPlayers().Count(r => r.IsHuman) > 0 && Player.GetPlayers().Count(r => r.IsSCP) > 0 &&
                   EventTime.TotalSeconds < 150 + 20);
        }

        protected override void OnFinished()
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.EffectsManager.EnableEffect<Flashed>(1);

                if (_heli != null)
                {
                    if (Vector3.Distance(player.Position, _finishPosition) > 5)
                    {
                        player.Damage(15000f, Translation.ZombieEscapeDied);
                    }
                }
            }

            if (Player.GetPlayers().Count(r => r.IsHuman) == 0)
            {
                Extensions.Broadcast(Translation.ZombieEscapeZombieWin, 10);
                Extensions.PlayAudio("ZombieWin.ogg", 7, false, Name);
            }
            else
            {
                Extensions.Broadcast(Translation.ZombieEscapeHumanWin, 10);
                Extensions.PlayAudio("HumanWin.ogg", 7, false, Name);
            }
        }
        protected override void OnCleanup()
        {
            Server.FriendlyFire = AutoEvent.IsFriendlyFireEnabledByDefault;
            if (_boat != null)
                Extensions.UnLoadMap(_boat);

            if (_heli != null)
                Extensions.UnLoadMap(_heli); 
        }

    }
}
