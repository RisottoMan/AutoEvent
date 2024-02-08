using MER.Lite.Objects;
using AutoEvent.Events.Handlers;
using MEC;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using InventorySystem.Items.Pickups;
using PluginAPI.Core.Items;
using UnityEngine;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Lava
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = "The floor is LAVA";
        public override string Description { get; set; } = "Survival, in which you need to avoid lava and shoot at others";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "lava";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventConfigPreset] 
        public Config OriginalLavaMap => Preset.Original;
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "Lava", 
            Position = new Vector3(120f, 1020f, -43.5f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "Lava.ogg", 
            Volume = 8, 
            Loop = false
        };
        private EventHandler _eventHandler { get; set; }
        private GameObject _lava;

        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;
        }

        protected override void UnregisterEvents()
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
        }

        private ItemType _getItemByChance()
        {
            if (Config.ItemsAndWeaponsToSpawn is not null && Config.ItemsAndWeaponsToSpawn.Count > 0)
            {
                if (Config.ItemsAndWeaponsToSpawn.Count == 1)
                {
                    return Config.ItemsAndWeaponsToSpawn.FirstOrDefault().Key;
                }

                List<KeyValuePair<ItemType, float>> list = Config.ItemsAndWeaponsToSpawn.ToList<KeyValuePair<ItemType, float>>();
                float roleTotalChance = list.Sum(x => x.Value);
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (UnityEngine.Random.Range(0, roleTotalChance) <= list[i].Value)
                    {
                        return list[i].Key;
                    }
                }

                return list[list.Count - 1].Key;
            }

            return ItemType.None;
        }
        protected override void OnStart()
        {
            if (Config.ItemsAndWeaponsToSpawn is not null && Config.ItemsAndWeaponsToSpawn.Count > 0)
            {
                DebugLogger.LogDebug($"Using Config for weapons.");
                List<Vector3> itemPositions = new List<Vector3>();
                foreach (var item in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
                {
                    if (item is null || item.Position.y < MapInfo.Position.y - 1)
                        continue;
                    itemPositions.Add(item.Position);
                    ItemPickup.Remove(item);
                    item.DestroySelf();
                }
                DebugLogger.LogDebug($"Positions found: {itemPositions.Count}");


                foreach (Vector3 position in itemPositions)
                {
                    ItemPickup.Create(_getItemByChance(), position + new Vector3(0,0.5f,0), Quaternion.Euler(Vector3.zero)).Spawn();
                }
            }


            foreach (var player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.Loadouts, LoadoutFlags.IgnoreGodMode);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast(Translation.Start.Replace("{time}", $"{time}"), 1);
                yield return Timing.WaitForSeconds(1f);
            }   
        }

        protected override void CountdownFinished()
        {
            _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "LavaObject");
            _lava.AddComponent<LavaComponent>();
            foreach (var player in Player.GetPlayers())
            {
                player.GiveInfiniteAmmo(AmmoMode.InfiniteAmmo);
            }
        }

        protected override bool IsRoundDone()
        {
            // If over one player is alive &&
            // Time is under 10 minutes (+ countdown)
            return !(Player.GetPlayers().Count(r => r.IsAlive) > 1 && EventTime.TotalSeconds < 600);
        }

        protected override void ProcessFrame()
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

            Extensions.Broadcast(text + Translation.Cycle.Replace("{count}", $"{Player.GetPlayers().Count(r => r.IsAlive)}"), 1);
            _lava.transform.position += new Vector3(0, 0.08f, 0);
        }

        protected override void OnFinished()
        {
            if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.Win.Replace("{winner}", Player.GetPlayers().First(r => r.IsAlive).Nickname), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.AllDead, 10);
            }
        }

    }
}
