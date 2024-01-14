using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Snowball;

public class Config : EventConfig
{
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

    //[Description("What item should give the players?")]
    //public ItemType ItemType { get; set; } = ItemType.Snowball;
}