using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Spleef;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Spleef", new Vector3(0f, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Spleef_Xmas2024", new Vector3(0f, 40f, 0f)), SeasonFlags.Christmas));
        }
    }
    
    [Description("How long the round should last.")]
    public int RoundDurationInSeconds { get; set; } = 120;

    [Description("The amount of health platforms have. Set to -1 to make them invincible.")]
    public float PlatformHealth { get; set; } = 1;

    [Description("A list of loadouts for spleef if a little count of players.")]
    public List<Loadout> PlayerLittleLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>() { ItemType.GunCom45 },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo,
        }
    };

    [Description("A list of loadouts for spleef if a normal count of players.")]
    public List<Loadout> PlayerNormalLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>() { ItemType.GunCOM18 },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    [Description("A list of loadouts for spleef if a big count of players.")]
    public List<Loadout> PlayerBigLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 },
            },
            Items = new List<ItemType>() { ItemType.GunCOM15 },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
}