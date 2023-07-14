using Exiled.API.Enums;
using Exiled.Events.EventArgs.Cassie;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;

namespace AutoEvent.Events.RandItems
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Scp173, SpawnReason.None, RoleSpawnFlags.All);
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
