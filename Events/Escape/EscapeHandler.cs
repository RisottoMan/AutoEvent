using Exiled.API.Enums;
using Exiled.Events.EventArgs.Cassie;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Warhead;
using PlayerRoles;

namespace AutoEvent.Events
{
    internal class EscapeHandler
    {
        public static void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Scp173, SpawnReason.None, RoleSpawnFlags.All);
        }
        public static void OnNukeDisable(StoppingEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public static void OnSendCassie(SendingCassieMessageEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
