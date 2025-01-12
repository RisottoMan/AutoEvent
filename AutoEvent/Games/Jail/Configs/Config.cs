using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace AutoEvent.Games.Jail;
public class Config : EventConfig
{
    [Description("How many lives each prisoner gets.")]
    public int PrisonerLives { get; set; } = 3;

    [Description("How many players will spawn as the jailors.")]
    public RoleCount JailorRoleCount { get; set; } = new(1, 4, 15f);
    
    [Description($"A list of loadouts for the jailors.")]
    public List<Loadout> JailorLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfCaptain, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR,
                ItemType.GunCOM18
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
        },
    };

    [Description("A list of loadouts for the prisoners.")]
    public List<Loadout> PrisonerLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 }
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
    
    [Description("What loadouts each locker can give.")]
    public List<Loadout> WeaponLockerLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            InfiniteAmmo = AmmoMode.InfiniteAmmo,
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR,
                ItemType.GunCOM15
            },
        },
        new Loadout()
        {
            InfiniteAmmo = AmmoMode.InfiniteAmmo,
            Items = new List<ItemType>()
            {
                ItemType.GunCrossvec,
                ItemType.GunRevolver
            }
        },
    };

    public List<Loadout> MedicalLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Health = 100
        }
    };
    
    public List<Loadout> AdrenalineLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            ArtificialHealth = new ArtificialHealth()
            {
                InitialAmount = 100f,
                MaxAmount = 100f,
                RegenerationAmount = 0,
                AbsorptionPercent = 70,
                Permanent = false,
                Duration = 0
            }
        }
    };
}