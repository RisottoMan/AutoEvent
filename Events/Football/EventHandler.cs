using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Events.Football
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
            /*
            if (Random.Range(0, 2) == 0)
            {
                ev.Player.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap, true);
            }
            else
            {
                ev.Player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap, false);
            }
            */
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
