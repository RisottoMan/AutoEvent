using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;

namespace AutoEvent.Events.Example
{
    // We can include events in the mini-game to perform various actions.
    public class EventHandler
    {
        // For example, a player joined into the game
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Scp173, SpawnReason.None, RoleSpawnFlags.All);
        }

        // Or, for example, if we don't want the players to be respawn
        public void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
