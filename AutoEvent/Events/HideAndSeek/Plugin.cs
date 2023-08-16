using AutoEvent.Events.HideAndSeek.Features;
using AutoEvent.Interfaces;
using CustomPlayerEffects;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.HideAndSeek
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.HideName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.HideDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "HideAndSeek";
        public override string CommandName { get; set; } = "hns";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurt;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll += _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBlood += _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.PickingUpItem += _eventHandler.OnPickUpItem;

            OnEventStarted();
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurt;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.SpawningRagdoll -= _eventHandler.OnSpawnRagdoll;
            Exiled.Events.Handlers.Map.PlacingBlood -= _eventHandler.OnPlaceBlood;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDropItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= _eventHandler.OnDropAmmo;
            Exiled.Events.Handlers.Player.PickingUpItem -= _eventHandler.OnPickUpItem;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap(MapName, new Vector3(5.5f, 1026.5f, -45f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("HideAndSeek.ogg", 5, true, Name);

            Server.FriendlyFire = true;

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.AssignInventory);
                player.Position = RandomClass.GetSpawnPosition(GameMap);

                player.EnableEffect<MovementBoost>();
                player.ChangeEffectIntensity<MovementBoost>(50);
            }

            Timing.RunCoroutine(OnEventRunning(), "hns_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            while (Player.List.Count(r => r.IsAlive) > 1)
            {
                for (float _time = 15; _time > 0; _time--)
                {
                    Extensions.Broadcast(AutoEvent.Singleton.Translation.HideBroadcast.Replace("%time%", $"{_time}"), 1);
                    yield return Timing.WaitForSeconds(1f);
                    EventTime += TimeSpan.FromSeconds(1f);
                }

                int catchCount = RandomClass.GetCatchByCount(Player.List.Count(r => r.IsAlive));
                for (int i = 0; i < catchCount; i++)
                {
                    var player = Player.List.Where(r => r.IsAlive && r.HasItem(ItemType.Jailbird) == false).ToList().RandomItem();
                    var item = player.AddItem(ItemType.Jailbird);
                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.CurrentItem = item;
                    });
                }

                for (int time = 15; time > 0; time--)
                {
                    Extensions.Broadcast(AutoEvent.Singleton.Translation.HideCycle.Replace("%time%", $"{time}"), 1);

                    yield return Timing.WaitForSeconds(1f);
                    EventTime += TimeSpan.FromSeconds(1f);
                }

                foreach (Player player in Player.List)
                {
                    if (player.HasItem(ItemType.Jailbird))
                    {
                        player.ClearInventory();
                        player.Hurt(200, AutoEvent.Singleton.Translation.HideHurt);
                    }
                }
            }

            OnEventEnded();
            yield break;
        }

        public void OnEventEnded()
        {
            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.HideMorePlayer.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                var text = AutoEvent.Singleton.Translation.HideOnePlayer;
                text = text.Replace("%winner%", Player.List.First(r => r.IsAlive).Nickname);
                text = text.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}");

                Extensions.Broadcast(text, 10);
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.HideAllDie.Replace("%time%", $"{EventTime.Minutes}:{EventTime.Seconds}"), 10);
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
