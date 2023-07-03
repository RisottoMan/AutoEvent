using AutoEvent.Events.HideAndSeek.Features;
using AutoEvent.Interfaces;
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
        public override string Name { get; set; } = "Догонялки [Testing]"; // Переработать патч
        public override string Description { get; set; } = "Надо догнать всех игроков на карте. [Testing]";
        public override string Color { get; set; } = "FF4242";
        public override string CommandName { get; set; } = "hide";
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
            //Exiled.Events.Handlers.Item.ChargingJailbird += _eventHandler.OnCharge;
            Exiled.Events.Handlers.Player.Shooting += _eventHandler.OnShooting;

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
            //Exiled.Events.Handlers.Item.ChargingJailbird -= _eventHandler.OnCharge;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            GameMap = Extensions.LoadMap("HideAndSeek", new Vector3(5.5f, 1026.5f, -45f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            Server.FriendlyFire = true;

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.AssignInventory);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
                // player.SetFriendlyFire(RoleTypeId.ClassD, 0);
            }

            Timing.RunCoroutine(OnEventRunning(), "hns_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (float _time = 15; _time > 0; _time--)
            {
                //Extensions.Broadcast($"<color=#D71868><b><i>{Name}</i></b></color>\n" +
                //    $"<color=#ABF000>БЕГИТЕ!\nОсталось <color=red>{_time}</color> секунд.</color>", 1);
                Extensions.Broadcast($"Выбор новых догоняющих игроков.\nДо начала осталось {_time} секунд.", 1);
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
                Log.Info(Player.List.Count(r => r.IsAlive && r.HasItem(ItemType.Jailbird) == false));
                var player = Player.List.Where(r => r.IsAlive && r.HasItem(ItemType.Jailbird) == false).ToList().RandomItem();
                Log.Info(player.Nickname);
                player.AddItem(ItemType.Jailbird);
            }

            for (int doptime = 10; doptime > 0; doptime--) // 30
            {
                Extensions.Broadcast($"Передайте биту другому игроку\n" +
                $"<color=yellow>Осталось <b><i>{doptime}</i></b> секунд!</color>", 1);
                //$"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            foreach(Player player in Player.List)
            {
                if (player.HasItem(ItemType.Jailbird))
                {
                    player.ClearInventory();
                    player.Hurt(200, "Вы не успели передать биту.");
                }
            }

            Timing.RunCoroutine(OnEventEnded(), "hns_run");
            yield break;
        }

        public IEnumerator<float> OnEventEnded()
        {
            if (Player.List.Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast($"Осталось много игроков.\n" +
                $"Ожидание перезагрузки.\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 10);

                yield return Timing.WaitForSeconds(10f);
                EventTime += TimeSpan.FromSeconds(10f);
                Timing.RunCoroutine(OnEventRunning(), "hns_end");
                yield break;
            }
            else if (Player.List.Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast($"Победил игрок {Player.List.First(r=>r.IsAlive).Nickname}\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 10);
            }
            else
            {
                Extensions.Broadcast($"Никто не выжил.\n" +
                $"Конец игры\n" +
                $"<color=yellow>Время ивента <color=red>{EventTime.Minutes}:{EventTime.Seconds}</color></color>", 10);
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
