using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Knives;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("35hp_2", new Vector3(0, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("35hp_2_Xmas2024", new Vector3(0, 40f, 0f)), SeasonFlags.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("35hp_2_Halloween2024", new Vector3(0, 40f, 0f)), SeasonFlags.Halloween));
        }
    }
    
    [Description("A list of loadouts that players on team 1 can get.")]
    public List<Loadout> Team1Loadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.NtfCaptain, 100 } },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0, 3) },
        }
    };

    [Description("A list of loadouts that players on team 2 can get.")]
    public List<Loadout> Team2Loadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ChaosRepressor, 100 } },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0, 3) },
        }
    };
}