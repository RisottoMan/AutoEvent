using Exiled.API.Enums;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using AutoEvent.Events.Lobby.Features;

namespace AutoEvent.Events.Lobby
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.ClassD, SpawnReason.None, RoleSpawnFlags.None);
            ev.Player.Position = RandomClass.GetSpawnPosition(_plugin.GameMap);
        }

        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHoleEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
