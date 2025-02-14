using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace AutoEvent.Games.Escape;
public class Config : EventConfig
{
    [Description("How long players have to escape in seconds. [Default: 70]")]
    public int EscapeDurationTime { get; set; } = 70;

    [Description("The time of the start and resume of the warhead in seconds. [Default: 100]")]
    public int EscapeResumeTime { get; set; } = 100;
    
    [Description("A list of loadouts for team Chaos Insurgency")]
    public List<Loadout> Scp173Loadout { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>(){ { RoleTypeId.Scp173, 100 } },
        }
    };
}