using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent.Games.Infection
{
    public class BossConfig
    {
        [Description("A list that contains maps. You also need to add maps to the Schematic folder. Specify the folder name of map.")]
        public List<string> ListOfMap { get; set; } = new List<string>()
        {
            "Battle",
        };
        
        [Description("A dictionary containing the name of the music and volume. They need to be manually added to the Music folder.")]
        public Dictionary<string, byte> ListOfMusic { get; set; } = new Dictionary<string, byte>()
        {
            ["Boss.ogg"] = 10,
        };
    }
}