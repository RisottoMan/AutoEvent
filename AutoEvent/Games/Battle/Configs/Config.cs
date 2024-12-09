using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Battle;
public class Config : EventConfig
{
    [Description("A List of Loadouts to use.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Health = 100,
            Chance = 33,
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR, ItemType.Medkit, ItemType.Medkit,
                ItemType.ArmorCombat, ItemType.SCP1853, ItemType.Adrenaline,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new Loadout()
        {
            Health = 115,
            Chance = 33,
            Items = new List<ItemType>()
            {
                ItemType.GunShotgun, ItemType.Medkit, ItemType.Medkit,
                ItemType.Medkit, ItemType.Medkit, ItemType.Medkit,
                ItemType.ArmorCombat, ItemType.SCP500,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new Loadout()
        {
            Health = 200,
            Chance = 33,
            Items = new List<ItemType>()
            {
                ItemType.GunLogicer, ItemType.ArmorHeavy, ItemType.SCP500,
                ItemType.SCP500, ItemType.SCP1853, ItemType.Medkit,
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}