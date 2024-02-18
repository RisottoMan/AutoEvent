using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Race;
public class Config : EventConfig
{
    [Description("How long the event should last in seconds. [Default: 60]")]
    public int EventDurationInSeconds = 60;

    [Description("A list of loadouts players can get.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } }
        }
    };
}