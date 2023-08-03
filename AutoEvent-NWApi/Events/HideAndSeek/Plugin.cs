using AutoEvent.Events.HideAndSeek.Features;
using CustomPlayerEffects;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Items;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Events.HideAndSeek
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = Translation.HideName;
        public override string Description { get; set; } = Translation.HideDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "HideAndSeek";
        public override string CommandName { get; set; } = "hns";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;
        public override void OnStart()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            OnEventStarted();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
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
                player.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);

                player.EffectsManager.EnableEffect<MovementBoost>();
                player.EffectsManager.ChangeState<MovementBoost>(50);
            }

            Timing.RunCoroutine(OnEventRunning(), "hns_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (float _time = 15; _time > 0; _time--)
            {
                Extensions.Broadcast(Translation.HideBroadcast.Replace("%time%", $"{_time}"), 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            int catchCount = 0;
            switch (Player.GetPlayers().Count(r => r.IsAlive))
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
                //var player = Player.GetPlayers().Where(r => r.IsAlive && r.HasItem(ItemType.Jailbird) == false).ToList().RandomItem();
                //var item = player.AddItem(ItemType.Jailbird);
                //Timing.CallDelayed(0.1f, () =>
                //{
                //    player.CurrentItem = item;
                //});
            }

            for (int doptime = 15; doptime > 0; doptime--)
            {
                Extensions.Broadcast(Translation.HideCycle.Replace("%time%", $"{doptime}"), 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            foreach(Player player in Player.GetPlayers())
            {
                /*
                if (player.Items.FirstOrDefault((Item r) => r.Type == ItemType.Jailbird))
                {
                    player.ClearInventory();
                    player.Damage(200, Translation.HideHurt);
                }
                */
            }

            Timing.RunCoroutine(OnEventEnded(), "hns_run");
            yield break;
        }

        public IEnumerator<float> OnEventEnded()
        {
            var time = $"{EventTime.Minutes}:{EventTime.Seconds}";
            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(Translation.HideMorePlayer.Replace("%time%", $"{time}"), 10);

                yield return Timing.WaitForSeconds(10f);
                EventTime += TimeSpan.FromSeconds(10f);

                Timing.RunCoroutine(OnEventRunning(), "hns_end");
                yield break;
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                var text = Translation.HideOnePlayer;
                text = text.Replace("%winner%", Player.GetPlayers().First(r => r.IsAlive).Nickname);
                text = text.Replace("%time%", $"{time}");

                Extensions.Broadcast(text, 10);
            }
            else
            {
                Extensions.Broadcast(Translation.HideAllDie.Replace("%time%", $"{time}"), 10);
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
