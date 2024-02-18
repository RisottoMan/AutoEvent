using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.AmongUs;
public class Config : EventConfig
{
    [Description("How long to wait before forcefully selecting a random player. Set to -1 to disable.")]
    public int AutoSelectDelayInSeconds { get; set; } = 10;
    
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