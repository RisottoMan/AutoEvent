using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.Escape
{
    public class EventHandler
    {
        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Scp173);
        }

        [PluginEvent(ServerEventType.CassieAnnouncesScpTermination)]
        public void OnSendCassie(CassieAnnouncesScpTerminationEvent ev)
        {
            //ev.IsAllowed = false;
        }

        public void OnTeamRespawn(TeamRespawnArgs ev)
        {
            ev.IsAllowed = false;
        }

        public void OnPlaceTantrum(PlaceTantrumArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
