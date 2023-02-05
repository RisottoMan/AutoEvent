using MEC;
using PlayerRoles;
using UnityEngine;
using AutoEvent.Events;

using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace AutoEvent
{
    internal class InfectionHandler
    {
        public static void OnDamage(HurtingEventArgs ev)
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
        public static void OnDead(DiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = InfectionEvent.GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
            });
        }
        public static void OnJoin(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                ev.Player.Position = InfectionEvent.GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
            });
        }
        public static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
