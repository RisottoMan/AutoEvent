using AutoEvent.Events.Infection;
using AutoEvent.Events.Jail;
using AutoEvent.Events.Versus;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent
{
    public class Config : IConfig
    {
        [Description("Enable/Disable AutoEvent.")]
        public bool IsEnabled { get; set; } = true;
        [Description("Enable/Disable Debug.")]
        public bool Debug { get; set; } = false;
        [Description("If you have donaters, then you can disable the admin panel for them during mini-games.")]
        public bool IsDisableDonators { get; set; } = true;
        [Description("List of donaters. Specify the GroupName from the RemoteAdmin config.")]
        public List<string> DonatorList { get; set; } = new List<string>()
        {
            "donate1"
        };
        
        [Description("The global volume of plugins (0 - 200, 100 is normal)")]
        public float Volume = 100;
        
        [Description("Infection config..")]
        public InfectionConfig InfectionConfig { get; set; } = new InfectionConfig();

        [Description("Enable/Disable jailbird ability..")]
        public bool IsJailbirdAbilityEnable { get; set; } = false;

        [Description("Enable/Disable infinity charges of jailbird..")]
        public bool IsJailbirdHasInfinityCharges { get; set; } = true;

    }
}
