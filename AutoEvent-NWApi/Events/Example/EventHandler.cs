using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Events.Example
{
    // We can include events in the mini-game to perform various actions.
    public class EventHandler
    {
        // For example, a player joined into the game

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Scp173, RoleChangeReason.None);
        }

        // Or, for example, if we don't want the players to be respawn

        [PluginEvent(ServerEventType.TeamRespawn)]
        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            //ev.IsAllowed = not working ooops;
        }
    }
}
