using System.Collections.Generic;
using System.ComponentModel;

namespace AutoEvent
{
    public class Config
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

        [Description("Enable/Disable jailbird ability..")]
        public bool IsJailbirdAbilityEnable { get; set; } = false;

        [Description("Enable/Disable infinity charges of jailbird..")]
        public bool IsJailbirdHasInfinityCharges { get; set; } = true;

    }
}
