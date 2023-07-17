using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Map;

namespace AutoEvent.Events.Jump
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev) => ev.Player.Role.Set(RoleTypeId.Spectator);
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
