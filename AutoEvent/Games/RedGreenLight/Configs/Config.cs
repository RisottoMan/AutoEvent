using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Light;

public class Config : EventConfig
{
    [Description("After how many seconds the round will end. [Default: 180]")]
    public int TotalTimeInSeconds { get; set; } = 60;

    [Description("A list of loadouts for team ClassD")]
    public List<Loadout> PlayerLoadout = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.ClassD, 100 } }
        }
    };
}