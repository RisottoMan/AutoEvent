using CustomPlayerEffects;
using MER.Lite.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using InventorySystem.Items.MarshmallowMan;
using InventorySystem.Items.ThrowableProjectiles;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.HideAndSeek
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent, IEventTag
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.HideTranslate.HideName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.HideTranslate.HideDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.HideTranslate.HideCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 1);

        [EventConfig]
        public HideAndSeekConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            { MapName = "HideAndSeek", Position = new Vector3(5.5f, 1026.5f, -45f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "HideAndSeek.ogg", Volume = 5, Loop = true };
        public TagInfo TagInfo { get; set; } = new TagInfo()
        {
            Name = "Xmas",
            Color = "#77dde7"
        };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private HideTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.HideTranslate;

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
            Server.FriendlyFire = true;

            foreach (Player player in Player.GetPlayers())
            {
                //Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.GiveLoadout(Config.PlayerLoadouts);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);

                // player.EffectsManager.EnableEffect<MovementBoost>();
                // player.EffectsManager.ChangeState<MovementBoost>(50);
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float _time = 15; _time > 0; _time--)
            {
                Extensions.Broadcast(Translation.HideBroadcast.Replace("{time}", $"{_time}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
        }

        protected override bool IsRoundDone()
        {
            return false;
        }


        private IEnumerator<float> PlayerBreak()
        {
            if (Config.BreakDuration < 1)
            {
                yield break;
            }
            // Wait for 15 seconds before choosing next batch.
            for (float _time = Config.BreakDuration; _time > 0; _time--)
            {
                Extensions.Broadcast(Translation.HideBroadcast.Replace("{time}", $"{_time}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
        }

        private IEnumerator<float> TagPeriod()
        {
            if (Config.TagDuration < 1)
            {
                yield break;
            }
            for (int time = Config.TagDuration; time > 0; time--)
            {
                Extensions.Broadcast(Translation.HideCycle.Replace("{time}", $"{time}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
        }

        private void SelectPlayers()
        {
            List<Player> playersToChoose = Player.GetPlayers().Where(x => x.IsAlive).ToList();
            foreach(Player ply in Config.TaggerCount.GetPlayers(true, playersToChoose))
            {
                ply.GiveLoadout(Config.TaggerLoadouts);
                var item = ply.AddItem(Config.TaggerWeapon);
                if (item.ItemTypeId == ItemType.SCP018)
                    item.MakeRock(new RockSettings(false, 1f, false, false, true));
                if (item.ItemTypeId == ItemType.GrenadeHE)
                    item.ExplodeOnCollision(true);
                Timing.CallDelayed(0.1f, () =>
                {
                    if (item != null)
                    {
                        ply.CurrentItem = item;
                    }
                });
            }

            if (Player.GetPlayers().Count(ply => ply.HasLoadout(Config.PlayerLoadouts)) <= Config.PlayersRequiredForBreachScannerEffect)
            {
                foreach(Player ply in Player.GetPlayers().Where(ply => ply.HasLoadout(Config.PlayerLoadouts)))
                {
                    ply.GiveEffect(StatusEffect.Scanned, 255, 0f, false);
                }
            }
        }
        protected override IEnumerator<float> RunGameCoroutine()
        {
            int playersAlive = Player.GetPlayers().Count(ply => ply.IsAlive && ply.HasLoadout(Config.PlayerLoadouts));
            while (DebugLogger.AntiEnd || playersAlive > 1)
            {
                SelectPlayers();
                
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(TagPeriod(), "TagPeriod"));

                // Kill players who are taggers.
                foreach (Player player in Player.GetPlayers())
                {
                    if (player.Items.Any(r => r.ItemTypeId == Config.TaggerWeapon))
                    {
                        player.ClearInventory();
                        player.Damage(200, Translation.HideHurt);
                    }
                }
                playersAlive = Player.GetPlayers().Count(ply => ply.IsAlive && ply.HasLoadout(Config.PlayerLoadouts));
                DebugLogger.LogDebug($"Players Alive: {playersAlive}");
                if (playersAlive <= 1)
                    break;
                
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(PlayerBreak(), "PlayerBreak"));
            }

            yield break;
        }

        protected override void OnFinished()
        {
            var translation = AutoEvent.Singleton.Translation.HideTranslate;

            /*if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(translation.HideMorePlayer.Replace("%time%", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }*/
             if (Player.GetPlayers().Count(r => r.IsAlive) >= 1)
            {
                var text = translation.HideOnePlayer;
                text = text.Replace("{winner}", Player.GetPlayers().First(r => r.IsAlive).Nickname);
                text = text.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

                Extensions.Broadcast(text, 10);
            }
            else
            {
                Extensions.Broadcast(translation.HideAllDie.Replace("{time}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }

        }
    }
}
