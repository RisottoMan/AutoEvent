using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Games.Battle;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Battle", new Vector3(0f, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Battle_Xmas2025", new Vector3(0f, 40f, 0f)), SeasonFlags.Christmas));
        }
    }
    
    [Description("A List of Loadouts to use.")]
    public List<Loadout> Loadouts { get; set; } = new()
    {
        new Loadout()
        {
            Health = 100,
            Chance = 33,
            Items = new()
            {
                ItemType.GunE11SR, ItemType.Medkit, ItemType.Medkit,
                ItemType.ArmorCombat, ItemType.SCP1853, ItemType.Adrenaline,
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new Loadout()
        {
            Health = 115,
            Chance = 33,
            Items = new()
            {
                ItemType.GunShotgun, ItemType.Medkit, ItemType.Medkit,
                ItemType.Medkit, ItemType.Medkit, ItemType.Medkit,
                ItemType.ArmorCombat, ItemType.SCP500,
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new Loadout()
        {
            Health = 200,
            Chance = 33,
            Items = new()
            {
                ItemType.GunLogicer, ItemType.ArmorHeavy, ItemType.SCP500,
                ItemType.SCP500, ItemType.SCP1853, ItemType.Medkit,
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}