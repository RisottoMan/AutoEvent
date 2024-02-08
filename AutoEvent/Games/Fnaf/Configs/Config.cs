using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Games.Fnaf.Features;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Fnaf;
public class Config : EventConfig
{
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

    public AnimatronicPreset FreddyPreset = new AnimatronicPreset()
    {
        Level = 3,
        Name = "Freddy",
        PositionName = "Freddy_Position",
        Timer = 5 // добавить фнафские числа
    };
}