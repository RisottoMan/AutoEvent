using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent.Events.Infection
{
    public sealed class SurvivalConfig
    {
        [Description("A list containing the name of the music. They need to be manually added to the Music folder.")]
        public List<string> ListOfMusic { get; set; } = new List<string>()
        {
            "Survival.ogg",
            "Survival1.ogg",
        };
    }
}