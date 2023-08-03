using PlayerRoles;

namespace AutoEvent.Events.Versus
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }
        /*
        public void OnDying(DyingEventArgs ev)
        {
            ev.Player.ClearInventory();

            if (ev.Player == _plugin.ClassD)
            {
                _plugin.ClassD = null;
            }
            if (ev.Player == _plugin.Scientist)
            {
                _plugin.Scientist = null;
            }
        }

        public void OnJoin(VerifiedEventArgs ev) => ev.Player.Role.Set(RoleTypeId.Spectator);
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlacingBulletHole ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
        */
    }
}
