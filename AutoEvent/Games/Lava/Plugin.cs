using MEC;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using UnityEngine;

namespace AutoEvent.Games.Lava;
public class Plugin : Event<Config, Translation>, IEventSound, IEventMap
{
    public override string Name { get; set; } = "The floor is LAVA";
    public override string Description { get; set; } = "Survival, in which you need to avoid lava and shoot at others";
    public override string Author { get; set; } = "RisottoMan";
    public override string CommandName { get; set; } = "lava";
    public MapInfo MapInfo { get; set; } = new()
    { 
        MapName = "Lava", 
        Position = new Vector3(120f, 1020f, -43.5f),
        IsStatic = false
    };
    public SoundInfo SoundInfo { get; set; } = new()
    { 
        SoundName = "Lava.ogg", 
        Volume = 8, 
        Loop = false
    };

    private EventHandler _eventHandler;
    private GameObject _lava;

    protected override void RegisterEvents()
    {
        _eventHandler = new EventHandler(this);
        Exiled.Events.Handlers.Player.Hurting += _eventHandler.OnHurting;
    }

    protected override void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.Hurting -= _eventHandler.OnHurting;
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

            List<KeyValuePair<ItemType, float>> list = Config.ItemsAndWeaponsToSpawn.ToList();
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
                // <<< An error occurred when creating the pickup. Maybe will fix it
                //ItemPickup.Create(_getItemByChance(), position + new Vector3(0,0.5f,0), Quaternion.Euler(Vector3.zero)).Spawn();
                // >>>
                if (!InventoryItemLoader.AvailableItems.TryGetValue(_getItemByChance(), out ItemBase itemBase))
                {
                    continue;
                }
                else
                {
                    PickupSyncInfo pickupSyncInfo = new PickupSyncInfo()
                    {
                        ItemId = _getItemByChance(),
                        Serial = ItemSerialGenerator.GenerateNext(),
                        WeightKg = itemBase.Weight
                    };
                    ItemPickupBase itemPickupBase = InventoryExtensions.ServerCreatePickup(
                        itemBase,
                        pickupSyncInfo, position + new Vector3(0,0.5f,0),
                        Quaternion.identity,
                        false,
                        null);
                    new ItemPickup(itemPickupBase).Spawn();
                }
                // >>>
            }
        }


        foreach (var player in Player.List)
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
        _lava.AddComponent<LavaComponent>().StartComponent(this);
        foreach (var player in Player.List)
        {
            player.GiveInfiniteAmmo(AmmoMode.InfiniteAmmo);
        }
    }

    protected override bool IsRoundDone()
    {
        // If over one player is alive &&
        // Time is under 10 minutes (+ countdown)
        return !(Player.List.Count(r => r.IsAlive) > 1 && EventTime.TotalSeconds < 600);
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

        Extensions.Broadcast(text + Translation.Cycle.Replace("{count}", $"{Player.List.Count(r => r.IsAlive)}"), 1);
        _lava.transform.position += new Vector3(0, 0.08f, 0);
    }

    protected override void OnFinished()
    {
        if (Player.List.Count(r => r.IsAlive) == 1)
        {
            Extensions.Broadcast(Translation.Win.Replace("{winner}", Player.List.First(r => r.IsAlive).Nickname), 10);
        }
        else
        {
            Extensions.Broadcast(Translation.AllDead, 10);
        }
    }
}