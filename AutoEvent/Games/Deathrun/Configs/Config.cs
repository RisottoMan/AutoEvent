using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace AutoEvent.Games.Deathrun;
public class Config : EventConfig
{
    [Description("How long the round should last in minutes.")]
    public int RoundDurationInSeconds { get; set; } = 300;
    [Description("How many seconds after the start of the game can be given a second life? Disable -> -1")]
    public int SecondLifeInSeconds { get; set; } = 15;
    
    [Description("Loadouts of run-guys")]
    public List<Loadout> PlayerLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
            Effects = new List<Effect>()
            {
                new Effect() { Type = EffectType.FogControl, Intensity = 1, Duration = 0 }
            },
            Chance = 100,
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    [Description("Loadouts of death-guys")]
    public List<Loadout> DeathLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scientist, 100 } },
            Effects = new List<Effect>()
            {
                new() { Type = EffectType.MovementBoost, Intensity = 50, Duration = 0 },
                new() { Type = EffectType.FogControl, Intensity = 1, Duration = 0 }
            },
            Chance = 100
        }
    };
    
    [Description("Weapon loadouts for finish")]
    public List<Loadout> WeaponLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Items = new List<ItemType>() { ItemType.GunE11SR, ItemType.Jailbird },
            Effects = new List<Effect>()
            {
                new() { Type = EffectType.MovementBoost, Intensity = 50, Duration = 0 }
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
    
    [Description("Poison loadouts")]
    public List<Loadout> PoisonLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Effects = new List<Effect>()
            {
                new() { Type = EffectType.CardiacArrest, Intensity = 1, Duration = 15 }
            },
        }
    };
}