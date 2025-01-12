using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.MusicalChairs;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("MusicalChairs", new Vector3(0f, 40f, 0f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("MusicalChairs_Xmas2024", new Vector3(0f, 40f, 0f)), SeasonFlags.Christmas));
        }
    }

    [Description("A loadout for players")]
    public List<Loadout> PlayerLoadout { get; set; } = new()
    {
        new Loadout()
        {
            Health = 100,
            Roles = new()
            {
                { RoleTypeId.ClassD, 50 },
                { RoleTypeId.Scientist, 50 }
            },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
            Stamina = 0
        },
    };
}