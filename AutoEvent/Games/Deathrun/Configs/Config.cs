using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Deathrun;
public class Config : EventConfig
{
    [Description("How long the round should last in minutes.")]
    public int RoundDurationInSeconds { get; set; } = 300;
    [Description("How many seconds after the start of the game can be given a second life? Disable -> -1")]
    public int SecondLifeInSeconds { get; set; } = 15;
    
    [Description("Loadouts of run-guys")]
    public List<Loadout> PlayerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
            Effects = new List<Effect>()
            {
                new Effect() { EffectType = StatusEffect.FogControl, Intensity = 1, Duration = 0 }
            },
            Chance = 100,
            InfiniteAmmo = AmmoMode.InfiniteAmmo
        }
    };

    [Description("Loadouts of death-guys")]
    public List<Loadout> DeathLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scientist, 100 } },
            Effects = new List<Effect>()
            {
                new Effect() { EffectType = StatusEffect.MovementBoost, Intensity = 50, Duration = 0 },
                new Effect() { EffectType = StatusEffect.FogControl, Intensity = 1, Duration = 0 }
            },
            Chance = 100
        }
    };
}