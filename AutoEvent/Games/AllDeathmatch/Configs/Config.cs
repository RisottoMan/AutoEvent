using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace AutoEvent.Games.AllDeathmatch;
public class Config : EventConfig
{
    [Description("How many minutes should we wait for the end of the round.")]
    public int TimeMinutesRound { get; set; } = 10;
    
    [Description("A list of loadouts for team NTF")]
    public List<Loadout> NTFLoadouts = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.NtfSpecialist, 100 } },
            Items = new List<ItemType>() { ItemType.ArmorCombat },
            InfiniteAmmo = AmmoMode.InfiniteAmmo,
            Effects = new List<Effect>()
            {
                new Effect()
                {
                    Type = EffectType.MovementBoost,
                    Intensity = 10,
                    Duration = 0,
                },
                new Effect()
                {
                    Type = EffectType.Scp1853,
                    Intensity = 1,
                    Duration = 0,
                }
            }
        }
    };

    [Description("The weapons a player can get once the round starts.")]
    public List<ItemType> AvailableWeapons = new List<ItemType>()
    {
        ItemType.GunAK,
        ItemType.GunCrossvec,
        ItemType.GunShotgun,
        ItemType.GunE11SR
    };
}