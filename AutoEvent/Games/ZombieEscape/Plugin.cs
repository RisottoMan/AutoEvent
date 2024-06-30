using MER.Lite.Objects;
using AutoEvent.Events.Handlers;
using CustomPlayerEffects;
using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.ZombieEscape
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        // todo - add a one way platform or way to fight back as mtf at end, once you are at the actual helicopter.
        public override string Name { get; set; } = "Zombie Escape";
        public override string Description { get; set; } = "Óou need to run away from zombies and escape by helicopter";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "zombie3";
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "zm_osprey", 
            Position = new Vector3(-15f, 1020f, -80f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Survival.ogg", 
            Volume = 10,
            Loop = false 
        };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler _eventHandler { get; set; }
        private GameObject _boat;
        private GameObject _heli;
        private GameObject _button;
        private GameObject _button1;
        private GameObject _button2;
        private GameObject _wall;
        private Vector3 _finishPosition;

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
            if (_eventHandler is null)
            {
                DebugLogger.LogDebug("Handler is null");
            }
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
        /*public Plugin(SchematicObject boat)
        {
            _boat = boat;
        }*/

        protected override void OnStart()
        {
            Server.FriendlyFire = false;
            foreach (Player player in Player.GetPlayers())
            {
                //Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                player.GiveLoadout(Config.MTFLoadouts);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);

                // Timing.CallDelayed(0.1f, () => { player.CurrentItem = item; });
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
                        break;
                    }
                    case "Button1":
                    {
                        _button1 = gameObject;
                        break;
                    }
                    case "Button2":
                    {
                        _button2 = gameObject;
                        break;
                    }
                    case "Lava":
                    {
                        gameObject.AddComponent<LavaComponent>();
                        break;
                    }
                    case "Wall":
                    {
                        _wall = gameObject;
                        break;
                    }
                    case "Finish":
                    {
                        _finishPosition = gameObject.transform.position;
                        break;
                    }
                    case "Helicopter":
                    {
                        _heli = gameObject;
                        if (gameObject.GetComponentInChildren<Animator>())
                        {
                            DebugLogger.LogDebug("Animator component for Helicopter is found", LogLevel.Debug);
                        }
                        break;
                    }
                }
            }
            DebugLogger.LogDebug("Serialized GameObjects.", LogLevel.Debug);
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float _time = 20; _time > 0; _time--)
            {
                Extensions.Broadcast(
                    Translation.Start.Replace("{name}", Name).Replace("{time}", $"{_time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            Extensions.PlayAudio("Zombie2.ogg", 7, false);

            foreach (Player ply in  Config.Zombies.GetPlayers(true))
            {
                DebugLogger.LogDebug($"{ply.Nickname} chosen as a zombie.", LogLevel.Debug);
                ply.GiveLoadout(Config.ZombieLoadouts);
                ply.EffectsManager.EnableEffect<Disabled>();
            }
        }

        protected override void ProcessFrame()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _button1.transform.position) < 3)
                {
                    _button1.transform.position += Vector3.down * 5;
                    _wall.AddComponent<WallComponent>().Init(Config.GateLockDuration);
                }

                var heliAnimation = _heli.GetComponentInChildren<Animator>();
                if (Vector3.Distance(player.Position, _button2.transform.position) < 3)
                {
                    _button2.transform.position += Vector3.down * 5;
                    EventTime = new TimeSpan(0, 0, (int)(Config.RoundDurationInSeconds - Config.EscapeDurationInSeconds));
                    heliAnimation.Play("Animation");
                }

                AnimatorClipInfo[] clipInfo = heliAnimation.GetCurrentAnimatorClipInfo(0);
                if (clipInfo[0].clip.name == "End")
                {
                    if (Vector3.Distance(player.Position, _finishPosition) > 5)
                    {
                        player.EffectsManager.EnableEffect<Flashed>(1);
                        player.Damage(15000f, Translation.Died);
                    }
                }
                
                string text = Translation.Helicopter.Replace("{name}", Name)
                    .Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsHuman)}");
                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);
            }

        }

        protected override bool IsRoundDone()
        {
            // At least 1 human player alive &&
            // At least 1 scp player alive && 
            // Elapsed time is shorter than 5 minutes (+ countdown)
            return !(Player.GetPlayers().Any(ply => Config.MTFLoadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ply.Role))) 
                     && Player.GetPlayers().Any(ply => Config.ZombieLoadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ply.Role))) &&
                   EventTime.TotalSeconds < Config.RoundDurationInSeconds);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.IsHuman) == 0)
            {
                Extensions.Broadcast(Translation.ZombieWin, 10);
                Extensions.PlayAudio("ZombieWin.ogg", 7, false);
            }
            else
            {
                Extensions.Broadcast(Translation.HumanWin, 10);
                Extensions.PlayAudio("HumanWin.ogg", 7, false);
            }
        }
    }
}
