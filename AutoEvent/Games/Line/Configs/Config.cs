using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using PlayerRoles;

namespace AutoEvent.Games.Line;
public class Config : EventConfig
{
    [Description("A list of loadouts players can get.")]
    public List<Loadout> Loadouts { get; set; } = new List<Loadout>()
    {
        new Loadout()
        {
            Roles = new Dictionary<RoleTypeId, int>()
            {
                { RoleTypeId.Scientist, 100 }
            }
        }
    };

    // public DifficultyItem LineDifficulty = new DifficultyItem();
    // public DifficultyItem WallDifficulty = new DifficultyItem();
    // public DifficultyItem DotsDifficulty = new DifficultyItem();
    // public DifficultyItem MiniWallsDifficulty = new DifficultyItem();
    /* todo
     * Eventually I hope to add difficulties to the objects 
     */
}