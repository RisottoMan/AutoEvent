using MER.Lite.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Example;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using InventorySystem.Items.MarshmallowMan;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Knives
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.KnivesTranslate.KnivesName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.KnivesTranslate.KnivesDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.KnivesTranslate.KnivesCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public KnivesConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "35hp_2", Position = new Vector3(5f, 1030f, -45f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Knife.ogg", Volume = 10, Loop = true };
        private EventHandler EventHandler { get; set; }
        private KnivesTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.KnivesTranslate;

        protected override void RegisterEvents()
        {

            EventHandler = new EventHandler();
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
            Server.FriendlyFire = false;

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (UnityEngine.Random.Range(0,2) == 1)
                {
                    player.GiveLoadout(Config.Team1Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                    // Extensions.SetRole(player, RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    player.GiveLoadout(Config.Team2Loadouts, LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
                    // Extensions.SetRole(player, RoleTypeId.ChaosRepressor, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }
                count++;

                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () => { player.CurrentItem = item; });
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

        protected override void CountdownFinished()
        {
            foreach(var wall in MapInfo.Map.AttachedBlocks.Where(x => x.name == "Wall"))
            {
                GameObject.Destroy(wall);
            }
        }

        protected override bool IsRoundDone()
        {
            // At least one NTF is alive &&
            // At least one Chaos is alive            
            return !(Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > 0 &&
                   Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) > 0);
        }

        protected override void ProcessFrame()
        {
            string mtfCount = Player.GetPlayers().Count(r => r.Team == Team.FoundationForces).ToString();
            string chaosCount = Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency).ToString();
            Extensions.Broadcast(Translation.KnivesCycle.
                Replace("{name}", Name).
                Replace("{mtfcount}", mtfCount).
                Replace("{chaoscount}", chaosCount), 1);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(Translation.KnivesChaosWin.Replace("{name}", Name), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(Translation.KnivesMtfWin.Replace("{name}", Name), 10);
            }
        }
    }
}
