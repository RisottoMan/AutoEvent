using PlayerRoles;
using PluginAPI.Events;

namespace AutoEvent.Games.Lobby
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Spectator);
        }
    }
}
