using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Snowball", new Vector3(0, 0, 30))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Dodgeball", new Vector3(0, 0, 30))));
        }
    }
    [Description("After how many seconds the round will end. [Default: 180]")]
    public int TotalTimeInSeconds { get; set; } = 180;

    [Description("A list of loadouts for team ClassD")]
    public List<Loadout> ClassDLoadouts = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.ClassD, 100 } }
        }
    };

    [Description("A list of loadouts for team Scientist")]
    public List<Loadout> ScientistLoadouts = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.Scientist, 100 } }
        }
    };
}