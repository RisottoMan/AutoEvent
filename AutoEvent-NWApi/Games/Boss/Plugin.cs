using AutoEvent.API.Schematic.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Boss
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "boss";
        public MapInfo MapInfo { get; set; } = new MapInfo() 
            { MapName = "DeathParty", Position = new Vector3(6f, 1030f, -43.5f) };

        public SoundInfo SoundInfo { get; set; } = new SoundInfo() 
            { SoundName = "Boss.ogg", Loop = false, Volume = 7, StartAutomatically = false };

        [EventConfig]
        public BossConfig Config { get; set; }
        private EventHandler EventHandler { get; set; }
        private BossTranslate Translation { get; set; }
        private Player _boss;

        protected override void RegisterEvents()
        {
            Translation = new BossTranslate();
            EventHandler = new EventHandler();
            
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += EventHandler.OnPlaceBullet;
            Servers.PlaceBlood += EventHandler.OnPlaceBlood;
            Players.DropItem += EventHandler.OnDropItem;
            Players.DropAmmo += EventHandler.OnDropAmmo;
        }

        protected override void OnStop()
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


        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(Translation.BossTimeLeft.Replace("{time}", $"{time}"), 5);
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }

        protected override bool IsRoundDone()
        {
            // Round Time is shorter than 2 minutes (+ 15 seconds for countdown)
            return !(EventTime.TotalSeconds < Config.DurationInSeconds + 15 
                   && EndConditions.TeamHasMoreThanXPlayers(Team.FoundationForces,0) 
                   && EndConditions.TeamHasMoreThanXPlayers(Team.ChaosInsurgency,0));
        }

        protected override void CountdownFinished()
        {
            StartAudio();
            // Lots of this is handled by the GiveLoadout() system now.
            _boss = Player.GetPlayers().Where(r => r.IsNTF).ToList().RandomItem();
            _boss.GiveLoadout(Config.BossLoadouts);
            //Extensions.SetRole(_boss, RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
            _boss.Position = RandomClass.GetSpawnPosition(MapInfo.Map);

            _boss.Health = Player.GetPlayers().Count() * 4000;
            // Extensions.SetPlayerScale(_boss, new Vector3(5, 5, 5));

            //_boss.ClearInventory();
            //_boss.AddItem(ItemType.GunLogicer);
            Timing.CallDelayed(0.1f, () =>
            {
                _boss.CurrentItem = _boss.Items.First();
            });
        }

        protected override void OnStart()
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.Loadouts);
                // Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                //player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
                // player.Health = 200;

                RandomClass.CreateSoldier(player);
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = player.Items.First(); });
            }

        }

        protected override void ProcessFrame()
        {
            string text = Translation.BossCounter;
            text = text.Replace("%hp%", $"{(int)_boss.Health}");
            text = text.Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsNTF)}");
            text = text.Replace("%time%", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            Extensions.SetPlayerScale(_boss, new Vector3(1, 1, 1));

            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.BossWin.Replace("%hp%", $"{(int)_boss.Health}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.BossHumansWin.Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsNTF)}"), 10);
            }
            
        }

    }
}
