using AutoEvent.Events;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.DeathParty
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Death Party";
        public override string Description { get; set; } = "Survive in grenade rain";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "deathparty";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public bool EventStarted;

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            OnEventStarted();
            EventStarted = true;
        }

        public override void OnStop()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;

            Timing.CallDelayed(5f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
            EventStarted = false;
        }

        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Player.List.ToList().ForEach(player =>
            {
                GameMap = Extensions.LoadMap("DeathParty", new Vector3(130f, 1012f, -40f), Quaternion.Euler(Vector3.zero), Vector3.one);
                player.Role.Set(RandomRoles.GetRandomRole());
                player.ClearInventory();
                player.Teleport(new Vector3(130f, 1013, -40f));
                player.Broadcast(5, "<color=red>Уворачивайтесь от гранат!</color>");
                player.EnableEffect(EffectType.Ensnared, 10);
                Server.FriendlyFire = true;
            });

            Extensions.PlayAudio("ClassicMusic.ogg", 100, true, "Death");

            Timing.RunCoroutine(OnEventRunning(), "death");
        }

        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"Уворачивайтесь от гранат.\nПрошло {EventTime}", 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
            }

            while (Player.List.Count(r => r.IsHuman) > 5 && Player.List.Count(r => r.IsAlive) > 0)
            {
                Extensions.Broadcast($"С начала ивента прошло {EventTime} секунд", 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
                Timing.CallDelayed(1, () =>
                {
                    GrenadeSpawn();
                });
            }

            foreach (Player player in Player.List)
            {
                player.Kill("End game");
            }
            Extensions.Broadcast($"Ивент закончен. Вы выжили {EventTime} секунд!", 10);

            OnStop();
            yield break;
        }

        public void GrenadeSpawn()
        {
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 2f;
            grenade.SpawnActive(new Vector3(130, 1023f, -40));
            grenade.Scale = new Vector3(3f, 3f, 3f);
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
            Extensions.UnLoadMap(GameMap);
            Server.FriendlyFire = false;
        }
    }
}
