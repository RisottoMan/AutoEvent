using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features.Objects;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;
using AutoEvent.Events.DeathParty.Features;
using Discord;
using Exiled.Events.EventArgs.Player;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.DeathParty
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Death Party";
        public override string Description { get; set; } = "Survive in grenade rain";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "death";
        public TimeSpan EventTime { get; set; }
        public SchematicObject GameMap { get; set; }
        public bool EventStarted;
        public int PlayerCount;
        public string EvWinner;
        public float ExplosionRadius;

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            OnEventStarted();
            EventStarted = true;
            ExplosionRadius = 12;
        }

        public override void OnStop()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Left -= OnLeft;

            Timing.CallDelayed(5f, EventEnd);
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
            EventStarted = false;
            ExplosionRadius = 0;
            EvWinner = null;
        }

        public void OnEventStarted()
        {
            Extensions.PlayAudio("Escape.ogg", 4, true, Name);
            GameMap = Extensions.LoadMap("DeathParty", new Vector3(130f, 1012f, -40f), Quaternion.Euler(Vector3.zero), Vector3.one);
            EventTime = new TimeSpan(0, 0, 0);
            Player.List.ToList().ForEach(player =>
            {
                player.Role.Set(RandomRoles.GetRandomRole());
                PlayerCount++;
                player.ClearInventory();
                player.Teleport(new Vector3(Random.Range(120, 140), 1015, Random.Range(-30, -50)));
                Server.FriendlyFire = true;
                Round.IsLocked = true;
                
            });

            Timing.RunCoroutine(OnEventRunning(), "death");
        }

        public IEnumerator<float> OnEventRunning()
        {
            while (PlayerCount >= 2)
            {
                Extensions.Broadcast($"<color=yellow>Уворачивайтесь от гранат!</color>\n<color=green>Прошло {EventTime} секунд</color>\n<color=red>Осталось {PlayerCount} человек</color>", 1);
                yield return Timing.WaitForSeconds(1f);
                EventTime += TimeSpan.FromSeconds(1f);
                Timing.CallDelayed(2f, GrenadeSpawn);
                if (PlayerCount == 1)
                {
                    foreach (Player ply in Player.List)
                    {
                        if (ply.IsAlive)
                        {
                            EvWinner = ply.DisplayNickname;
                            ply.IsGodModeEnabled = true;
                            Extensions.Broadcast($"<color=red>Победил {EvWinner}</color>", 5);
                            Timing.CallDelayed(5f, () =>
                            {
                                OnStop();
                                ply.IsGodModeEnabled = false;
                            });
                        }
                    }
                } 
                else if (PlayerCount <= 0)
                {
                    Extensions.Broadcast($"<color=red>Победитель неопределен</color>", 5);
                    Timing.CallDelayed(5f, OnStop);
                }

                if (PlayerCount < 5)
                {
                    ExplosionRadius = 20f;
                }
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            ev.Player.ShowHint($"<color=yellow>Вы смогли выжить {EventTime} секунд</color>", 5f);
            PlayerCount--;
        }

        public void OnLeft(LeftEventArgs ev)
        {
            PlayerCount--;
        }

        public void GrenadeSpawn()
        {
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.Scale = new Vector3(2, 3, 2);
            grenade.FuseTime = 1.5f;
            grenade.MaxRadius = ExplosionRadius;
            grenade.SpawnActive(new Vector3(Random.Range(110f, 150f), 1020f, Random.Range(-20f, -60f)));
        }

        public void EventEnd()
        {
            Server.FriendlyFire = false;
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
            Extensions.UnLoadMap(GameMap);
        }
    }
}
