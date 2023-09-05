using MEC;
using PlayerRoles;

using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Map;
using Exiled.API.Features;
using System.Linq;

namespace AutoEvent.Events.FallDown
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHoleEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
