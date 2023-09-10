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
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Survival
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.SurvivalTranslate.SurvivalName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.SurvivalTranslate.SurvivalDescription;
        public override string Author { get; set; } = "KoT0XleB";
        
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Survival", Position = new Vector3(15f, 1030f, -43.68f), Rotation = Quaternion.identity};
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "DeathParty.ogg", Volume = 5, Loop = true };
        public override string CommandName { get; set; } = "zombie2";
        private bool isFriendlyFireEnabled { get; set; }
        public Player firstZombie { get; set; }
        EventHandler EventHandler { get; set; }
        protected override float PostRoundDelay { get; set; } = 10f;
        private SurvivalTranslate Translation { get; set; }
        private GameObject Teleport { get; set; }
        private GameObject Teleport1 { get; set; }

        protected override void OnStart()
        {
            Translation = new SurvivalTranslate();
            isFriendlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;
            PrepareEvent();

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

        protected override void OnStop()
        {
            Server.FriendlyFire = isFriendlyFireEnabled;

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

        public void PrepareEvent()
        {
            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
                //Extensions.SetPlayerAhp(player, 100, 100, 0);

                var item = player.AddItem(RandomClass.GetRandomGun());
                player.AddItem(ItemType.GunCOM18);
                player.AddItem(ItemType.ArmorCombat);

                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = item;
                });
            }
            Extensions.StopAudio();
            Timing.CallDelayed(0.1f, () =>
            {
                Extensions.PlayAudio("Zombie2.ogg", 7, false, Name);
            });
            for (int i = 0; i <= Player.GetPlayers().Count() / 10; i++)
            {
                var player = Player.GetPlayers().Where(r => r.IsHuman).ToList().RandomItem();
                Extensions.SetRole(player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                player.EffectsManager.EnableEffect<Disabled>();
                player.EffectsManager.EnableEffect<Scp1853>();
                player.Health = 5000;

                if (Player.GetPlayers().Count(r => r.IsSCP) == 1)
                {
                    firstZombie = player;
                }
            }

            Teleport = MapInfo.Map.AttachedBlocks.First(x => x.name == "Teleport");
            Teleport1 = MapInfo.Map.AttachedBlocks.First(x => x.name == "Teleport1");
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (float _time = 20; _time > 0; _time--)
            {
                Extensions.Broadcast(Translation.SurvivalBeforeInfection.Replace("%name%", Name).Replace("%time%", $"{_time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override bool IsRoundDone()
        {
            return Player.GetPlayers().Count(r => r.IsHuman) > 0 && Player.GetPlayers().Count(r => r.IsSCP) > 0 &&
                EventTime.TotalSeconds > 0;
        }

        protected override void ProcessFrame()
        {
            var text = Translation.SurvivalAfterInfection;
            text = text.Replace("%name%", Name);
            text = text.Replace("%humanCount%", Player.GetPlayers().Count(r => r.IsHuman).ToString());
            text = text.Replace("%time%", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

            foreach (var player in Player.GetPlayers())
            {
                player.ClearBroadcasts();
                player.SendBroadcast(text, 1);

                if (Vector3.Distance(player.Position, Teleport.transform.position) < 1)
                {
                    player.Position = Teleport1.transform.position;
                }
            }
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.IsHuman) == 0)
            {
                Extensions.Broadcast(Translation.SurvivalZombieWin, 10);
                Extensions.PlayAudio("ZombieWin.ogg", 7, false, Name);
            }
            else if (Player.GetPlayers().Count(r => r.IsSCP) == 0)
            {
                Extensions.Broadcast(Translation.SurvivalHumanWin, 10);
                Extensions.PlayAudio("HumanWin.ogg", 7, false, Name);
            }
            else
            {
                Extensions.Broadcast(Translation.SurvivalHumanWinTime, 10);
                Extensions.PlayAudio("HumanWin.ogg", 7, false, Name);
            }
        }
    }
}
