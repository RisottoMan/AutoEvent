using System.Collections.Generic;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Infection;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Zombie", new Vector3(0, 40f, 0f) )));
            AvailableMaps.Add(new MapChance(50, new MapInfo("ZombieRework", new Vector3(0, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Zombie_Xmas2024", new Vector3(0, 40f, 0f)), SeasonFlags.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("ZombieRework_Xmas2024", new Vector3(0, 40f, 0f)), SeasonFlags.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Zombie_Halloween2024", new Vector3(0, 40f, 0f)), SeasonFlags.Halloween));
            AvailableMaps.Add(new MapChance(50, new MapInfo("ZombieRework_Halloween2024", new Vector3(0, 40f, 0f)), SeasonFlags.Halloween));
        }
    }

    public List<Loadout> PlayerLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
        }
    };

    public List<Loadout> ZombieLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scp0492, 100 } },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
        }
    };
    
    public List<string> ZombieScreams { get; set; } = new()
    {
        "human_death_01.ogg",
        "human_death_02.ogg"
    };
}