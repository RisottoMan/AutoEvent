using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Events.Escape
{
    public class EventHandler
    {
        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Scp173, RoleChangeReason.None);
        }

        [PluginEvent(ServerEventType.CassieAnnouncesScpTermination)]
        public void OnSendCassie(CassieAnnouncesScpTerminationEvent ev)
        {
            //ev.IsAllowed = false;
        }

        [PluginEvent(ServerEventType.TeamRespawn)]
        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            //ev.IsAllowed = go f...ck dude;
        }

        [PluginEvent(ServerEventType.Scp173CreateTantrum)]
        public void OnPlaceTantrum(Scp173CreateTantrumEvent ev)
        {
            //ev.IsAllowed = false;  WHHYYYYYYYYYYYYYYYYYYYYYYY
        }
    }
}
