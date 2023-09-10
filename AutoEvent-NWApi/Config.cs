using AutoEvent.Games.Infection;
using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent
{
#if EXILED
    public class Config : Exiled.API.Interfaces.IConfig
#else
    public class Config
#endif
    {
        [Description("Enable/Disable AutoEvent.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable/Disable Debug.")]
        public bool Debug { get; set; } = false;

        [Description("If you have donaters, then you can disable the admin panel for them during mini-games.")]
        public bool IsDisableDonators { get; set; } = true;

        [Description("A list of admins who can run mini-games. Specify the GroupName from the config_remoteadmin")]
        public List<string> PermissionList { get; set; } = new List<string>()
        {
            "owner",
            "admin",
            "moderator"
        };

        [Description("List of donaters. Specify the GroupName from the config_remoteadmin")]
        public List<string> DonatorList { get; set; } = new List<string>()
        {
            "donate1"
        };

        [Description("The global volume of plugins (0 - 200, 100 is normal)")]
        public float Volume = 100;

        public InfectConfig InfectConfig { get; set; } = new InfectConfig();
    }
}
