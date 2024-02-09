using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Games.Versus;
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
            AvailableMaps.Add(new MapChance(50, new MapInfo("35Hp", new Vector3(6f, 1015f, -5f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("35Hp_Xmas2024", new Vector3(6f, 1015f, -5f)), SeasonFlag.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("35Hp_Xmas2024", new Vector3(6f, 1015f, -5f)), SeasonFlag.NewYear));
        }
    }

    [Description("Can be used to disable the jailbird charging attack.")]
    public bool JailbirdCanCharge { get; set; } = false;
    
    [Description("How long to wait before forcefully selecting a random player. Set to -1 to disable.")]
    public int AutoSelectDelayInSeconds { get; set; } = 10;
    
    [Description("Enables the halloween effect melee instead of jailbird.")]
    public bool HalloweenMelee { get; set; } = true;
    
    [Description("Loadouts for team 1.")]
    public List<Loadout> Team1Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.Scientist, 100 } },
        }
    };
    [Description("Loadouts for team 2.")]
    public List<Loadout> Team2Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
        }
    };
}