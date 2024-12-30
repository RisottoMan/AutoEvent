using MEC;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
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
        MapName = "Lava_Remake", 
        Position = new Vector3(120f, 1020f, -43.5f),
        IsStatic = false // ?
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

    protected override void OnStart()
    {
        var spawnpoints = new List<GameObject>();
        var spawnguns = new List<GameObject>();
        
        foreach (var obj in MapInfo.Map.AttachedBlocks)
        {
            switch (obj.name)
            {
                case "Spawnpoint": spawnpoints.Add(obj); break;
                case "Spawngun": spawnguns.Add(obj); break;
            }
        }
        
        if (Config.ItemsAndWeaponsToSpawn is not null && Config.ItemsAndWeaponsToSpawn.Count > 0)
        {
            foreach (var goGun in spawnguns)
            {
                ItemType itemType = ItemType.None;
                
                if (Config.ItemsAndWeaponsToSpawn.Count == 1)
                {
                    itemType = Config.ItemsAndWeaponsToSpawn.FirstOrDefault().Key;
                }

                var list = Config.ItemsAndWeaponsToSpawn.ToList();
                float roleTotalChance = list.Sum(x => x.Value);
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (Random.Range(0, roleTotalChance) <= list[i].Value)
                    {
                        itemType = list[i].Key;
                    }
                }

                Pickup pickup = Pickup.CreateAndSpawn(itemType, goGun.transform.position);
                MapInfo.Map.AttachedBlocks.Add(pickup.GameObject);
            }
        }
        
        foreach (var player in Player.List)
        {
            player.GiveLoadout(Config.Loadouts, LoadoutFlags.IgnoreGodMode);
            player.Position = spawnpoints.RandomItem().transform.position;
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