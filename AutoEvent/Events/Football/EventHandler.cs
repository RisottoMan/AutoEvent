using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;

namespace AutoEvent.Events.Football
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }
        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
