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
            "donate4",
            "donate3",
            "donate2",
            "donate1"
        };
    }
}
