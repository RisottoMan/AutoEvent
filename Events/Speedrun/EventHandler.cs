using MEC;
using PlayerRoles;

using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Map;
using Exiled.API.Features;
using System.Linq;

namespace AutoEvent.Events.Speedrun
{
    public class EventHandler
    {
        public void OnDamage(HurtingEventArgs ev)
        {
            //
        }

        public void OnDead(DiedEventArgs ev)
        {
            //
        }

        public void OnJoin(VerifiedEventArgs ev)
        {
            //
        }

        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
