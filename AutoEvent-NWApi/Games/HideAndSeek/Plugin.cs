using CustomPlayerEffects;
using AutoEvent.API.Schematic.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using InventorySystem.Items.ThrowableProjectiles;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.HideAndSeek
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.HideTranslate.HideName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.HideTranslate.HideDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "hns";

        [EventConfig]
        public HideAndSeekConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "HideAndSeek", Position = new Vector3(5.5f, 1026.5f, -45f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "HideAndSeek.ogg", Volume = 5, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private HideTranslate Translation { get; set; }

        protected override void RegisterEvents()
        {
            Translation = new HideTranslate();
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
                Extensions.Broadcast(Translation.HideBroadcast.Replace("%time%", $"{_time}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }
        }

        protected override bool IsRoundDone()
        {
            return false;
        }

        protected override IEnumerator<float> RunGameCoroutine()
        {
            for (float _time = 15; _time > 0; _time--)
            {
                Extensions.Broadcast(Translation.HideBroadcast.Replace("%time%", $"{_time}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            int catchCount = RandomClass.GetCatchByCount(Player.GetPlayers().Count(r => r.IsAlive));
            for (int i = 0; i < catchCount; i++)
            {
                var player = Player.GetPlayers().Where(r => r.IsAlive &&
                                                            r.Items.Any(r => r.ItemTypeId == Config.TaggerWeapon) ==
                                                            false).ToList().RandomItem();
               player.GiveLoadout(Config.TaggerLoadouts);
                var item = player.AddItem(Config.TaggerWeapon);
                //var scp018 = player.AddItem(ItemType.SCP018);
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = item; });
            }

            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.HideCycle.Replace("%time%", $"{time}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            foreach (Player player in Player.GetPlayers())
            {
                if (player.Items.Any(r => r.ItemTypeId == Config.TaggerWeapon))
                {
                    player.ClearInventory();
                    player.Damage(200, Translation.HideHurt);
                }
            }
        }

        protected override void OnFinished()
        {
            var translation = AutoEvent.Singleton.Translation.HideTranslate;

            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(translation.HideMorePlayer.Replace("%time%", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                var text = translation.HideOnePlayer;
                text = text.Replace("%winner%", Player.GetPlayers().First(r => r.IsAlive).Nickname);
                text = text.Replace("%time%", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

                Extensions.Broadcast(text, 10);
            }
            else
            {
                Extensions.Broadcast(translation.HideAllDie.Replace("%time%", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}"), 10);
            }

        }

        protected override void OnCleanup()
        {
            Server.FriendlyFire = AutoEvent.IsFriendlyFireEnabledByDefault;
        }
    }
}
