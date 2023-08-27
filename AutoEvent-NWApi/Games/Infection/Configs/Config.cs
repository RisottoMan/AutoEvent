using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent.Games.Infection
{
    public class InfectionConfig
    {
        [Description("A list that contains maps. You also need to add maps to the Schematic folder. Specify the folder name of map.")]
        public static List<string> ListOfMap { get; set; } = new List<string>()
        {
            "Zombie",
            "ZombieRework",
        };
        
        [Description("A dictionary containing the name of the music and volume. They need to be manually added to the Music folder.")]
        public static Dictionary<string, byte> ListOfMusic { get; set; } = new Dictionary<string, byte>()
        {
            ["Zombie.ogg"] = 7,
            ["Zombie2.ogg"] = 7,
        };
    }
}