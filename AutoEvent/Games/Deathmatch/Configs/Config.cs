using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Deathmatch;
public class Config : EventConfig
{
    public Config()
    {
        if (AvailableMaps is null)
        {
            AvailableMaps = new List<MapChance>();
        }

        if (AvailableMaps.Count < 1)
        {
            AvailableMaps.Add(new MapChance(50, new MapInfo("Shipment", new Vector3(0, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Shipment_Xmas2025", new Vector3(0, 40f, 0f)), SeasonFlags.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Shipment_Halloween2024", new Vector3(0, 40f, 0f)), SeasonFlags.Halloween));
        }
    }
    
    [Description("How many total kills a team needs to win. Determined per-person at the start of the round. [Default: 3]")]
    public int KillsPerPerson { get; set; } = 3;

    [Description("A list of loadouts for team Chaos Insurgency")]
    public List<Loadout> ChaosLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.ChaosRifleman, 100 } },
            Items = new List<ItemType>() { ItemType.ArmorCombat, ItemType.Medkit, ItemType.Painkillers },
            InfiniteAmmo = AmmoMode.InfiniteAmmo,
            Effects = new List<Effect>()
            {
                new (EffectType.MovementBoost, 10, 0),
                new (EffectType.Scp1853, 1, 0),
                new (EffectType.FogControl, 0)
            }
        }
    };
    
    [Description("A list of loadouts for team NTF")]
    public List<Loadout> NTFLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.NtfSpecialist, 100 } },
            Items = new List<ItemType>() { ItemType.ArmorCombat, ItemType.Medkit, ItemType.Painkillers },
            InfiniteAmmo = AmmoMode.InfiniteAmmo,
            Effects = new List<Effect>()
            {
                new (EffectType.MovementBoost, 10, 0),
                new (EffectType.Scp1853, 1, 0),
                new (EffectType.FogControl, 0)
            }
        }
    };

    [Description("The weapons a player can get once the round starts.")]
    public List<ItemType> AvailableWeapons { get; set; } = new()
    {
        ItemType.GunAK,
        ItemType.GunCrossvec,
        ItemType.GunFSP9,
        ItemType.GunE11SR
    };
}