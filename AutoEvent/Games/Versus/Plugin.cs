using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using MEC;
using PlayerRoles;
using MER.Lite.Objects;
using UnityEngine;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Hints;
using InventorySystem.Items.MarshmallowMan;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Versus
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.VersusTranslate.VersusName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.VersusTranslate.VersusDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = AutoEvent.Singleton.Translation.VersusTranslate.VersusCommandName;
        public override Version Version { get; set; } = new Version(1, 0, 0);
        [EventConfig]
        public VersusConfig Config { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "35Hp", Position = new Vector3(6f, 1015f, -5f), };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "Knife.ogg", Volume = 10, Loop = true };
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Disable;
        private EventHandler EventHandler { get; set; }
        private VersusTranslate Translation { get; set; } = AutoEvent.Singleton.Translation.VersusTranslate;
        public Player Scientist { get; set; }
        public Player ClassD { get; set; }
        private List<GameObject> _triggers;
        private List<GameObject> _teleports;
        private TimeSpan _countdown;

        public override void InstantiateEvent()
        {
            if(Config.Team1Loadouts.Any(loadout => loadout.Roles.Any(role => Config.Team2Loadouts.Any(loadout2 => loadout2.Roles.Any(role2 => role.Key == role2.Key)))))
            {
                DebugLogger.LogDebug($"{Name} has two enemy team roles that are the same role. This will break the event if it is run. To prevent this, default configs will be used for loadouts.");
                var newConf = new VersusConfig();
                Config.Team1Loadouts = newConf.Team1Loadouts;
                Config.Team2Loadouts = newConf.Team2Loadouts;
            }
        }

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
            Players.ChargingJailbird += EventHandler.OnJailbirdCharge;
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
            Players.ChargingJailbird -= EventHandler.OnJailbirdCharge;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            Scientist = null;
            ClassD = null;
            
            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (UnityEngine.Random.Range(0,2) == 1)
                {
                    player.GiveLoadout(Config.Team1Loadouts);
                    //Extensions.SetRole(player, RoleTypeId.Scientist, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, true);
                }
                else
                {
                    player.GiveLoadout(Config.Team2Loadouts);
                    // Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(MapInfo.Map, false);
                }
                count++;

                if (Config.HalloweenMelee)
                {
                    player.EffectsManager.EnableEffect<MarshmallowEffect>();
                }
                else
                {
                    var item = player.AddItem(ItemType.Jailbird);
                    Timing.CallDelayed(0.2f, () => { player.CurrentItem = item; });
                }
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
            _triggers = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Trigger").ToList();
            _teleports = MapInfo.Map.AttachedBlocks.Where(x => x.name == "Teleport").ToList();
            _countdown = new TimeSpan(0, 0, Config.AutoSelectDelayInSeconds);
        }

        protected override bool IsRoundDone()
        {
            // At least 1 player on scientists &&
            // At least 1 player on dbois
            return !(Player.GetPlayers().Any(ply => Config.Team1Loadouts.Any(loadout => loadout.Roles.Any(role => ply.Role == role.Key))) &&
                     Player.GetPlayers().Any(ply => Config.Team2Loadouts.Any(loadout => loadout.Roles.Any(role => ply.Role == role.Key))));
        }

        protected override void ProcessFrame()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (Scientist == null)
                {
                    if (Config.Team1Loadouts.Any(x => x.Roles.Any(y => y.Key == player.Role)) &&
                        (Vector3.Distance(player.Position, _triggers.ElementAt(0).transform.position) <= 1f ||
                         (Config.AutoSelectDelayInSeconds != -1 && _countdown.TotalSeconds == 0)))
                    {
                        Scientist = player;
                        Scientist.Position = _teleports.ElementAt(0).transform.position;
                        if (ClassD != null)
                            ClassD.Heal(100);

                        _countdown = new TimeSpan(0, 0,  Config.AutoSelectDelayInSeconds);
                    }
                }

                if (ClassD == null)
                {
                    if (Config.Team2Loadouts.Any(x => x.Roles.Any(y => y.Key == player.Role)) &&
                        (Vector3.Distance(player.Position, _triggers.ElementAt(1).transform.position) <= 1f ||
                         (Config.AutoSelectDelayInSeconds != -1 && _countdown.TotalSeconds == 0)))
                    {
                        ClassD = player;
                        ClassD.Position = _teleports.ElementAt(1).transform.position;
                        if (Scientist != null)
                            Scientist.Heal(100);

                        _countdown = new TimeSpan(0, 0, Config.AutoSelectDelayInSeconds);
                    }
                }
            }

            if (ClassD == null && Scientist == null)
            {
                Extensions.Broadcast(
                    Translation.VersusPlayersNull.Replace("{name}", Name)
                        .Replace("{remain}", $"{_countdown.TotalSeconds}"), 1);
            }
            else if (ClassD == null)
            {
                Extensions.Broadcast(
                    Translation.VersusClassDNull.Replace("{name}", Name).Replace("{scientist}", Scientist.Nickname)
                        .Replace("{remain}", $"{_countdown.TotalSeconds}"), 1);
            }
            else if (Scientist == null)
            {
                Extensions.Broadcast(
                    Translation.VersusScientistNull.Replace("{name}", Name).Replace("{classd}", ClassD.Nickname)
                        .Replace("{remain}", $"{_countdown.TotalSeconds}"), 1);
            }
            else
            {
                Extensions.Broadcast(
                    Translation.VersusPlayersDuel.Replace("{name}", Name).Replace("{scientist}", Scientist.Nickname)
                        .Replace("{classd}", ClassD.Nickname), 1);
            }

            _countdown = _countdown.Subtract(new TimeSpan(0, 0, 1));
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scientist) == 0)
            {
                Extensions.Broadcast(Translation.VersusClassDWin.Replace("{name}", Name), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.ClassD) == 0)
            {
                Extensions.Broadcast(Translation.VersusScientistWin.Replace("{name}", Name), 10);
            }
        }
        
    }
}
