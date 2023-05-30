using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Item;

namespace AutoEvent.Events.Knifes
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }
        public void OnDropItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public void OnChargeJailbird(ChargingJailbirdEventArgs ev)
        {
            var item = (Jailbird)ev.Item;
            item.Base._chargeDuration = 0;
        }
    }
}
