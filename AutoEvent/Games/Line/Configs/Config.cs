using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Line;
public class Config : EventConfig
{
    [Description("A list of loadouts players can get.")]
    public List<Loadout> Loadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.Scientist, 100 }
            }
        }
    };
    
    [Description("A list of loadouts players can get.")]
    public List<Loadout> FailureLoadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.ClassD, 100 }
            }
        }
    };
}