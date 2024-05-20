using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Trouble;
public class Config : EventConfig
{
    [Description("The loadouts that players can get.")]
    public List<Loadout> PlayerLoadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>() { { RoleTypeId.ClassD, 100 } },
            Items = new List<ItemType>() { ItemType.SCP207, ItemType.SCP500 }
        },
    };
}

