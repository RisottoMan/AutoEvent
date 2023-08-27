using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Lava
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.LavaTranslate.LavaName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.LavaTranslate.LavaDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Lava";
        public override string CommandName { get; set; } = "lava";
        private bool isFreindlyFireEnabled { get; set; }
        TimeSpan EventTime { get; set; }
        SchematicObject GameMap { get; set; }
        EventHandler _eventHandler { get; set; }

        public override void OnStart()
        {
            isFreindlyFireEnabled = Server.FriendlyFire;
            Server.FriendlyFire = false;

            OnEventStarted();

            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;
        }

        public override void OnStop()
        {
            Server.FriendlyFire = isFreindlyFireEnabled;

            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PlayerDamage -= _eventHandler.OnPlayerDamage;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);

            GameMap = Extensions.LoadMap(MapName, new Vector3(120f, 1020f, -43.5f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("Lava.ogg", 7, false, Name);

            foreach (var player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
            }

            Timing.RunCoroutine(OnEventRunning(), "lava_time");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation.LavaTranslate;

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(translation.LavaBeforeStart.Replace("%time%", $"{time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }

            GameObject lava = GameMap.AttachedBlocks.First(x => x.name == "LavaObject");
            lava.AddComponent<LavaComponent>();

            while (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                string text = string.Empty;
                if (EventTime.TotalSeconds % 2 == 0)
                {
                    text = "<size=90><color=red><b>《 ! 》</b></color></size>\n";
                }
                else
                {
                    text = "<size=90><color=red><b>!</b></color></size>\n";
                }

                Extensions.Broadcast(text + translation.LavaCycle.Replace("%count%", $"{Player.GetPlayers().Count(r => r.IsAlive)}"), 1);
                lava.transform.position += new Vector3(0, 0.08f, 0);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(translation.LavaWin.Replace("%winner%", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(translation.LavaAllDead, 10);
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
