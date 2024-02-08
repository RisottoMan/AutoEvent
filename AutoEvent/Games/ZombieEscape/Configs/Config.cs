using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.ZombieEscape;

public class Config : EventConfig
{
    [Description("How long until the helicopter leaves.")]
    public float EscapeDurationInSeconds { get; set; } = 65;

    [Description("How long the MTF have to escape.")]
    public float RoundDurationInSeconds { get; set; } = 150;
    [Description("How long the gate locks for.")]
    public float GateLockDuration { get; set; } = 15f;
    
    [Description("The loadouts that MTF can get.")]
    public List<Loadout> MTFLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } },
            Items = new List<ItemType>() { ItemType.GunAK, ItemType.GunCOM18, ItemType.ArmorCombat },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        },
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfSergeant, 100 } },
            Items = new List<ItemType>() { ItemType.GunE11SR, ItemType.GunCOM18, ItemType.ArmorCombat },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    [Description("The loadouts that Zombies can get.")]
    public List<Loadout> ZombieLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scp0492, 100 } },
            Effects = new List<Effect>() { new Effect() { EffectType = StatusEffect.Scp1853 }, new Effect() { EffectType = StatusEffect.Disabled, Duration = 5 } },
            Health = 10000,
        }
    };

    [Description("The amount of Zombies that can spawn.")]
    public RoleCount Zombies { get; set; } = new RoleCount()
    {
        MinimumPlayers = 1,
        MaximumPlayers = 3,
        PlayerPercentage = 10,
    };

    public WeaponEffect WeaponEffect { get; set; } = new WeaponEffect()
    {
        Damage = 3f,
        Effects = new List<Effect>()
        {
            new Effect()
            {
                EffectType = StatusEffect.Concussed,
                Duration = 0.5f,
                AddDuration = true
            },
            new Effect()
            {
                EffectType = StatusEffect.Disabled,
                Duration = 0.5f,
                AddDuration = true
            },
        }
    };
}

