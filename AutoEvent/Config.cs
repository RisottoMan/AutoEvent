using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using PlayerRoles;

namespace AutoEvent;
public class Config : Exiled.API.Interfaces.IConfig
{
    public Config()
    {
        string basePath = Path.Combine(Exiled.API.Features.Paths.Configs, "AutoEvent");
        SchematicsDirectoryPath = Path.Combine(basePath, "Schematics");
        MusicDirectoryPath = Path.Combine(basePath, "Music");
    }

    [Description("Enable/Disable AutoEvent.")]
    public bool IsEnabled { get; set; } = true;

    [Description("Enable/Disable Debug.")]
    public bool Debug { get; set; } = false;
    
    [Description("Enables / Disables Auto-Logging to a debug output file. Enabled by default on debug releases.")]
    public bool AutoLogDebug { get; set; } = false;
    
    [Description("The global volume of plugins (0 - 200, 100 is normal)")]
    public float Volume { get; set; } = 100;

    [Description("Roles that should be ignored during events.")]
    public List<RoleTypeId> IgnoredRoles { get; set; } = new()
    {
        RoleTypeId.Tutorial,
        RoleTypeId.Overwatch,
        RoleTypeId.Filmmaker
    };

    [Description("The players will be set once an event is done. **DO NOT USE A ROLE THAT IS ALSO IN IgnoredRoles**")]
    public RoleTypeId LobbyRole { get; set; } = RoleTypeId.ClassD;
    
    [Description("Where the schematics directory is located. By default it is located in the AutoEvent folder.")]
    public string SchematicsDirectoryPath { get; set; }
    
    [Description("Where the music directory is located. By default it is located in the AutoEvent folder.")]
    public string MusicDirectoryPath { get; set; }
}