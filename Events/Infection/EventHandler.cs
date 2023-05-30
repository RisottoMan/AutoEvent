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
        }
        public void OnDead(DiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = Plugin.GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
            });
        }
        public void OnJoin(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = Plugin.GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
            });
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) { ev.IsAllowed = false; }
    }
}
