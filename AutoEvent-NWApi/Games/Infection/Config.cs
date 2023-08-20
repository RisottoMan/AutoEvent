using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent.Games.Infection
{
    public sealed class InfectionConfig
    {
        [Description("Enable/Disable fall damage for people.")]
        public static bool FallDamageEnabled { get; set; } = false;
        [Description("A list that contains maps. You also need to add maps to the Schematic folder. Specify the folder name of map.")]
        public static List<string> ListOfMap { get; set; } = new List<string>()
        {
            "Zombie",
            "ZombieRework",
        };
        [Description("A list containing the name of the music. They need to be manually added to the Music folder.")]
        public static List<string> ListOfMusic { get; set; } = new List<string>()
        {
            "Zombie.ogg",
            "Zombie2.ogg",
        };
    }
}