using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Enums;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Lava;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("Lava", new Vector3(120f, 1020f, -43.5f), null, null, false)));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Lava_Xmas2024", new Vector3(120f, 1020f, -43.5f), null, null, false), SeasonFlags.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Lava_Xmas2024", new Vector3(120f, 1020f, -43.5f), null, null, false), SeasonFlags.NewYear));
        }
    }

    [Description("Can players drop guns.")]
    public bool PlayersCanDropGuns { get; set; } = true;
    [Description("A list of available loadouts that players can get.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } }
        }
    };

    [Description("A list of weapons / items that will spawn around the map, as well as the chance of that object spawning. If you leave this blank, the default map spawns will be used.")]
    public Dictionary<ItemType, float> ItemsAndWeaponsToSpawn { get; set; } = new Dictionary<ItemType, float>()
    {
        { ItemType.GunCOM15,     25 },  // 25 
        { ItemType.GunCOM18,     25 },  // 50
        { ItemType.GunRevolver,  20 },  // 70
        { ItemType.GunFSP9,      13 },  // 83
        { ItemType.GunShotgun,   07 },  // 90
        { ItemType.GunCrossvec,  07 },  // 97
        { ItemType.GunAK,        03 },  // 100
    };
}
