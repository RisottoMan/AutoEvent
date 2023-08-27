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
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Boss
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.BossTranslate.BossDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "DeathParty";
        public override string CommandName { get; set; } = "boss";
        TimeSpan EventTime { get; set; }
        SchematicObject GameMap { get; set; }
        EventHandler _eventHandler { get; set; }
        Player Boss { get; set; }
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
            EventTime = new TimeSpan(0, 2, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(6f, 1030f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.NtfSergeant, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
                player.Health = 200;

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
            var translation = AutoEvent.Singleton.Translation.BossTranslate;
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast(translation.BossTimeLeft.Replace("{time}", $"{time}"), 5);
                yield return Timing.WaitForSeconds(1f);
            }

            Extensions.PlayAudio("Boss.ogg", 7, false, Name);

            Boss = Player.GetPlayers().Where(r => r.IsNTF).ToList().RandomItem();
            Extensions.SetRole(Boss, RoleTypeId.ChaosConscript, RoleSpawnFlags.None);
            Boss.Position = RandomClass.GetSpawnPosition(GameMap);

            Boss.Health = Player.GetPlayers().Count() * 4000;
            Extensions.SetPlayerScale(Boss, new Vector3(5, 5, 5));

            Boss.ClearInventory();
            Boss.AddItem(ItemType.GunLogicer);
            Timing.CallDelayed(0.1f, () =>
            {
                Boss.CurrentItem = Boss.Items.First();
            });

            while (EventTime.TotalSeconds > 0 && Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) > 0 && Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) > 0)
            {
                var text = translation.BossCounter;
                text = text.Replace("%hp%", $"{(int)Boss.Health}");
                text = text.Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsNTF)}");
                text = text.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}");

                Extensions.Broadcast(text, 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            Extensions.SetPlayerScale(Boss, new Vector3(1, 1, 1));

            if (Player.GetPlayers().Count(r => r.Team == Team.FoundationForces) == 0)
            {
                Extensions.Broadcast(translation.BossWin.Replace("%hp%", $"{(int)Boss.Health}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.Team == Team.ChaosInsurgency) == 0)
            {
                Extensions.Broadcast(translation.BossHumansWin.Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsNTF)}"), 10);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
