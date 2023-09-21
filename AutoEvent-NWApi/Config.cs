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
        
#if !EXILED
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
#endif

        [Description("The global volume of plugins (0 - 200, 100 is normal)")]
        public float Volume = 100;
        
#if !EXILED
        [Description("Where the configs directory is located. By default it is located in the AutoEvent folder.")]
        public string EventConfigsDirectoryPath = "/home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Configs";
        
        [Description("Where the external events directory is located. By default it is located in the AutoEvent folder.")]
        public string ExternalEventsDirectoryPath = "/home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Events";
        
        [Description("Where the schematics directory is located. By default it is located in the AutoEvent folder.")]
        public string SchematicsDirectoryPath = "/home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Schematics";
        
        [Description("Where the music directory is located. By default it is located in the AutoEvent folder.")]
        public string MusicDirectoryPath = "/home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/AutoEvent/Music";
#else        
        [Description("Where the configs directory is located. By default it is located in the AutoEvent folder.")]
        public string EventConfigsDirectoryPath = "/home/container/.config/EXILED/Configs/AutoEvent/Configs";
        
        [Description("Where the external events directory is located. By default it is located in the AutoEvent folder.")]
        public string ExternalEventsDirectoryPath = "/home/container/.config/EXILED/Configs/AutoEvent/Events";
        
        [Description("Where the schematics directory is located. By default it is located in the AutoEvent folder.")]
        public string SchematicsDirectoryPath = "/home/container/.config/EXILED/Configs/AutoEvent/Schematics";
        
        [Description("Where the music directory is located. By default it is located in the AutoEvent folder.")]
        public string MusicDirectoryPath = "/home/container/.config/EXILED/Configs/AutoEvent/Music";

#endif        
    }
}
