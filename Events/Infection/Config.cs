using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent.Events.Infection
{
    public sealed class InfectionConfig
    {
        [Description("Enable/Disable fall damage for people.")]
        public bool FallDamageEnabled { get; set; } = false;
        [Description("Enable/Disable fall damage for people.")]
        public List<string> ListOfMusic { get; set; } = new List<string>()
        {
            "Zombie.ogg",
            "Zombie2.ogg",
        };
    }
}