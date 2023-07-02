using MEC;
using PlayerRoles;

using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Map;
using Exiled.API.Features;
using System.Linq;

namespace AutoEvent.Events.Infection
{
    public class EventHandler
    {
        public void OnDamage(HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Attacker.Role == RoleTypeId.Scp0492)
                {
                    ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.None);
                    ev.Attacker.ShowHitMarker();
                }
            }
            else if (!AutoEvent.Singleton.Config.InfectionConfig.FallDamageEnabled && ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnDead(DiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            });
        }

        public void OnJoin(VerifiedEventArgs ev)
        {
            if (Player.List.Count(r => r.Role.Type == RoleTypeId.Scp0492) > 0)
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }
            else
            {
                ev.Player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }
        }

        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
