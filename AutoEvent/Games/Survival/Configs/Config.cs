using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Survival;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Survival", new Vector3(0f, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Survival_Xmas2025", new Vector3(0f, 40f, 0f)), SeasonFlags.Christmas));
        }
    }
    
    [Description("How long the round should last in seconds.")]
    public int RoundDurationInSeconds { get; set; } = 300;
    
    [Description("A list of lodaouts players can get.")]
    public List<Loadout> PlayerLoadouts { get; set; } = new()
    {
        new()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunAK,
                ItemType.GunCOM18,
                ItemType.ArmorCombat,
            },
            ArtificialHealth = new ArtificialHealth()
            {
                InitialAmount = 100,
                MaxAmount = 100,
                Duration = 0
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR,
                ItemType.GunCOM18,
                ItemType.ArmorCombat,
            },
            ArtificialHealth = new ArtificialHealth()
            {
                InitialAmount = 100,
                MaxAmount = 100,
                Duration = 0
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
    
    [Description("A list of loadouts zombies can get.")]
    public List<Loadout> ZombieLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scp0492, 100 } },
            Effects = new List<Effect>()
            {
                new(EffectType.Disabled, 0),
                new(EffectType.Scp1853, 0),
                new(EffectType.FogControl, 0)
            },
            Health = 2000
        }
    };
    
    [Description("The amount of Zombies that can spawn.")]
    public RoleCount Zombies { get; set; } = new()
    {
        MinimumPlayers = 1,
        MaximumPlayers = 3,
        PlayerPercentage = 10,
    };
    
    [Description("Zombie screams sounds.")]
    public List<string> ZombieScreams { get; set; } = new()
    {
        "human_death_01.ogg",
        "human_death_02.ogg"
    };
}