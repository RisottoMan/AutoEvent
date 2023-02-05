using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;

namespace AutoEvent.Events
{
    internal class VersusHandler
    {
        public static void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }
        public static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public static void OnDead(DiedEventArgs ev)
        {
            if (ev.Player == VersusEvent.ClassD)
            {
                VersusEvent.ClassD = null;
            }
            if (ev.Player == VersusEvent.Scientist)
            {
                VersusEvent.Scientist = null;
            }
        }
    }
}
