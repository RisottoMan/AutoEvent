using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace AutoEvent.Games.Race;
public class Config : EventConfig
{
    [Description("How long the event should last in seconds. [Default: 60]")]
    public int EventDurationInSeconds { get; set; } = 60;

    [Description("A list of loadouts players can get.")]
    public List<Loadout> Loadouts { get; set; } = new()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
            Effects = new List<Effect>() { new(EffectType.FogControl, 0) },
        },
    };
}