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
            for (float _time = 15; _time > 0; _time--)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.HideBroadcast.Replace("%time%", $"{_time}"), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            int catchCount = 0;
            switch (Player.List.Count(r => r.IsAlive))
            {
                case int n when (n > 0 && n <= 3): catchCount = 1; break;
                case int n when (n > 3  && n <= 5): catchCount = 2; break;
                case int n when (n > 5 && n <= 10): catchCount = 3; break;
                case int n when (n > 10 && n <= 15): catchCount = 5; break;
                case int n when (n > 15 && n <= 20): catchCount = 8; break;
                case int n when (n > 20 && n <= 25): catchCount = 10; break;
                case int n when (n > 25): catchCount = n / 2; break;
            }

            for(int i = 0; i < catchCount; i++)
            {
                var player = Player.List.Where(r => r.IsAlive && r.HasItem(ItemType.Jailbird) == false).ToList().RandomItem();
                var item = player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () =>
                {
                    player.CurrentItem = item;
                });
            }

            for (int doptime = 15; doptime > 0; doptime--)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.HideCycle.Replace("%time%", $"{doptime}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            foreach(Player player in Player.List)
            {
                if (player.HasItem(ItemType.Jailbird))
                {
                    player.ClearInventory();
                    player.Hurt(200, AutoEvent.Singleton.Translation.HideHurt);
                }
            }

            Timing.RunCoroutine(OnEventEnded(), "hns_end");
            yield break;
        }

        public IEnumerator<float> OnEventEnded()
        {
            var time = $"{EventTime.Minutes}:{EventTime.Seconds}";
            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.HideMorePlayer.Replace("%time%", $"{time}"), 10);

                yield return Timing.WaitForSeconds(10f);
                EventTime += TimeSpan.FromSeconds(10f);

                Timing.RunCoroutine(OnEventRunning(), "hns_run");
                yield break;
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                var text = AutoEvent.Singleton.Translation.HideOnePlayer;
                text = text.Replace("%winner%", Player.List.First(r => r.IsAlive).Nickname);
                text = text.Replace("%time%", $"{time}");

                Extensions.Broadcast(text, 10);
            }
            else
            {
                Extensions.Broadcast(AutoEvent.Singleton.Translation.HideAllDie.Replace("%time%", $"{time}"), 10);
            }

            OnStop();
            yield break;
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
