using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.CounterStrike;
public class Config : EventConfig
{
    [Description("After how many seconds the round will end. [Default: 105]")]
    public int TotalTimeInSeconds { get; set; } = 105;
    
    [Description("A list of loadouts for team NTF")]
    public List<Loadout> NTFLoadouts = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.NtfSpecialist, 100 } },
            Items = new List<ItemType>() { ItemType.GunE11SR, ItemType.GrenadeHE, ItemType.GrenadeFlash, ItemType.Radio, ItemType.ArmorCombat }
        }
    };

    [Description("A list of loadouts for team Chaos Insurgency")]
    public List<Loadout> ChaosLoadouts = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.ChaosRifleman, 100 } },
            Items = new List<ItemType>() { ItemType.GunAK, ItemType.GrenadeHE, ItemType.GrenadeFlash, ItemType.Radio, ItemType.ArmorCombat }
        }
    };
}