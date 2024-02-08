using System.Collections.Generic;
using AutoEvent.API;

namespace AutoEvent.Games.Lava;
public static class Preset
{
    public static Config Original = new Config()
    {
        ItemsAndWeaponsToSpawn = new Dictionary<ItemType, float>(),
        GunEffects = new List<WeaponEffect>(),
    };
}