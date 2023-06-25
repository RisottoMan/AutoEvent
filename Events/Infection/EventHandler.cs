using MEC;
using PlayerRoles;
using UnityEngine;

using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

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
                    ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                    ev.Attacker.ShowHitMarker();
                }
            }
            else if (!AutoEvent.Singleton.Config.InfectionConfig.FallDamageEnabled
                && ev.DamageHandler.Type == Exiled.API.Enums.DamageType.Falldown) ev.IsAllowed = false;
        }
        public void OnDead(DiedEventArgs ev)
        {
            if (ev.DamageHandler.Type != Exiled.API.Enums.DamageType.Falldown) return;

            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
            });
        }
        public void OnJoin(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            });
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) { ev.IsAllowed = false; }
    }
}
