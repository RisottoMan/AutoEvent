using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("MusicalChairs", new Vector3(115.5f, 1030f, -43.5f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("MusicalChairs_Xmas2024", new Vector3(115.5f, 1030f, -43.5f)), SeasonFlag.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("MusicalChairs_Xmas2024", new Vector3(115.5f, 1030f, -43.5f)), SeasonFlag.NewYear));
        }
    }

    [Description("A loadout for players")]
    public List<Loadout> PlayerLoadout = new List<Loadout>()
    {
        new Loadout()
        {
            Health = 100,
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 50 },
                { RoleTypeId.Scientist, 50 }
            }
        },
    };
}