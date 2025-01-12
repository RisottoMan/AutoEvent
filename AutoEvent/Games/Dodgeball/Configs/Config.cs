using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Dodgeball;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Dodgeball", new Vector3(0, 0, 30))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Snowball", new Vector3(0, 0, 30)), SeasonFlags.Christmas));
        }
    }
    [Description("After how many seconds the round will end. [Default: 180]")]
    public int TotalTimeInSeconds { get; set; } = 180;

    [Description("A list of loadouts for team ClassD")]
    public List<Loadout> ClassDLoadouts = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.ClassD, 100 } },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
        }
    };

    [Description("A list of loadouts for team Scientist")]
    public List<Loadout> ScientistLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.Scientist, 100 } },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
        }
    };
}