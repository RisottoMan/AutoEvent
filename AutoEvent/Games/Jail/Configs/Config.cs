using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using YamlDotNet.Serialization;

namespace AutoEvent.Games.Jail;
public class Config : EventConfig
{
    [Description("How many lives each prisoner gets.")]
    public int PrisonerLives { get; set; } = 3;

    [Description("How many players will spawn as the jailors.")]
    public RoleCount JailorRoleCount { get; set; } = new RoleCount(1, 4, 15f);
    
    [Description($"A list of loadouts for the jailors.")]
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
    public List<Loadout> JailorLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfCaptain, 100 } },
            Items = new List<ItemType>()
            {
                ItemType.GunE11SR,
                ItemType.GunCOM18
            },
            Effects = new List<Effect>()
            {
                new Effect(EffectType.FogControl, 0, 3)
            }
        },
    };

    [Description("A list of loadouts for the prisoners.")]
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
    public List<Loadout> PrisonerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 }
            },
            Effects = new List<Effect>()
            {
                new Effect(EffectType.FogControl, 0, 3)
            },
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };
    
    [Description("What loadouts each locker can give.")]
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
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

    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
    public List<Loadout> MedicalLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Health = 100
        }
    };
    
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)]
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