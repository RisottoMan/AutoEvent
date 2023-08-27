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
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.HideAndSeek
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.HideTranslate.HideName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.HideTranslate.HideDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "HideAndSeek";
        public override string CommandName { get; set; } = "hns";
        SchematicObject GameMap { get; set; }
        TimeSpan EventTime { get; set; }
        EventHandler _eventHandler { get; set; }

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
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;

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
            Players.PlayerDamage -= _eventHandler.OnPlayerDamage;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(5.5f, 1026.5f, -45f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("HideAndSeek.ogg", 5, true, Name);

            Server.FriendlyFire = true;

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);

                player.EffectsManager.EnableEffect<MovementBoost>();
                player.EffectsManager.ChangeState<MovementBoost>(50);
            }

            Timing.RunCoroutine(OnEventRunning(), "hns_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation.HideTranslate;

            while (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                for (float _time = 15; _time > 0; _time--)
                {
                    Extensions.Broadcast(translation.HideBroadcast.Replace("%time%", $"{_time}"), 1);

                    yield return Timing.WaitForSeconds(1f);
                    EventTime += TimeSpan.FromSeconds(1f);
                }

                int catchCount = RandomClass.GetCatchByCount(Player.GetPlayers().Count(r => r.IsAlive));
                for (int i = 0; i < catchCount; i++)
                {
                    var player = Player.GetPlayers().Where(r => r.IsAlive && 
                    r.Items.Any(r => r.ItemTypeId == ItemType.Jailbird) == false).ToList().RandomItem();
                    var item = player.AddItem(ItemType.Jailbird);

                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.CurrentItem = item;
                    });
                }

                for (int time = 15; time > 0; time--)
                {
                    Extensions.Broadcast(translation.HideCycle.Replace("%time%", $"{time}"), 1);

                    yield return Timing.WaitForSeconds(1f);
                    EventTime += TimeSpan.FromSeconds(1f);
                }

                foreach (Player player in Player.GetPlayers())
                {
                    if (player.Items.Any(r => r.ItemTypeId == ItemType.Jailbird))
                    {
                        player.ClearInventory();
                        player.Damage(200, translation.HideHurt);
                    }
                }
            }

            OnEventEnded();
            yield break;
        }

        public void OnEventEnded()
        {
            var translation = AutoEvent.Singleton.Translation.HideTranslate;

            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(translation.HideMorePlayer.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                var text = translation.HideOnePlayer;
                text = text.Replace("%winner%", Player.GetPlayers().First(r => r.IsAlive).Nickname);
                text = text.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}");

                Extensions.Broadcast(text, 10);
            }
            else
            {
                Extensions.Broadcast(translation.HideAllDie.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
            }

            OnStop();
        }

        public void EventEnd()
        {
            Server.FriendlyFire = false;
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
