using MEC;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace AutoEvent.Events.Breakout
{
    public class EventHandler
    {
        public void OnDamage(HurtingEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (ev.Attacker.Role == RoleTypeId.Scp0492)
                {
                    ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.AssignInventory);
                    ev.Attacker.ShowHitMarker();
                }
            }
        }
        public void OnDead(DiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.AssignInventory);
                //ev.Player.Position = _gameMap.Position + RandomClass.GetRandomSpawn();
            });
        }
        public void OnJoin(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.AssignInventory);
                //ev.Player.Position = _gameMap.Position + RandomClass.GetRandomSpawn();
            });
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
