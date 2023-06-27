using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using System.Collections.Generic;

namespace AutoEvent.Events.HideAndSeek
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }
        public void OnDamage(HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Attacker.Role == RoleTypeId.NtfSergeant)
                {
                    ev.Player.Role.Set(RoleTypeId.NtfSergeant, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                    ev.Player.ResetInventory(new List<ItemType> { ItemType.Jailbird });
                }
            }
        }
        public void OnDead(DiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.NtfSergeant, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                //ev.Player.Position = InfectionEvent.GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
                ev.Player.ResetInventory(new List<ItemType> { ItemType.Jailbird });
            });
        }
        public void OnJoin(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.NtfSergeant, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                //ev.Player.Position = InfectionEvent.GameMap.transform.position + new Vector3(-18.75f, 2.5f, 0f);
                ev.Player.ResetInventory(new List<ItemType> { ItemType.Jailbird });
            });
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) { ev.IsAllowed = false; }
    }
}
