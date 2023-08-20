using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using AutoEvent.Games.Battle.Features;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Battle
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.BattleName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.BattleDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Battle";
        public override string CommandName { get; set; } = "battle";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public List<GameObject> Workstations { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;

            OnEventStarted();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(6f, 1030f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("MetalGearSolid.ogg", 10, false, Name);

            var count = 0;
            foreach (Player player in Player.GetPlayers())
            {
                if (count % 2 == 0)
                {
                    Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, true);
                }
                else
                {
                    Extensions.SetRole(player, RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
                    player.Position = RandomClass.GetSpawnPosition(GameMap, false);
                }
                count++;

                RandomClass.CreateSoldier(player);
                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = player.Items.First();
                });
            }

            Timing.RunCoroutine(OnEventRunning(), "battle_time");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;
            for (int time = 20; time > 0; time--)
            {
                Extensions.Broadcast($"{translation.BattleTimeLeft.Replace("{time}", $"{time}")}", 5);
                yield return Timing.WaitForSeconds(1f);
            }

            Workstations = new List<GameObject>();
            foreach (var gameObject in GameMap.AttachedBlocks)
            {
                switch (gameObject.name)
                {
                    case "Wall": { GameObject.Destroy(gameObject); } break;
                    case "Workstation": { Workstations.Add(gameObject); } break;
                }
            }

            while (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > 0 && Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) > 0)
            {
                var text = translation.BattleCounter;
                text = text.Replace("{FoundationForces}", $"{Player.GetPlayers().Count(r => r.Team == Team.FoundationForces)}");
                text = text.Replace("{ChaosForces}", $"{Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency)}");
                text = text.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}");

                Extensions.Broadcast(text, 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast($"{translation.BattleCiWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}")}", 3);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(translation.BattleMtfWin.Replace("{time}", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            foreach (var bench in Workstations) GameObject.Destroy(bench);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
