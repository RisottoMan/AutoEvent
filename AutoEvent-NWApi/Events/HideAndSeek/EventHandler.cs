using PlayerRoles;

namespace AutoEvent.Events.HideAndSeek
{
    public class EventHandler
    {
        /*
        public void OnHurt(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.IsAllowed = false;
            }

            if (ev.Attacker != null)
            if (ev.Attacker.HasItem(ItemType.Jailbird) == true && ev.Player.HasItem(ItemType.Jailbird) == false)
            {
                ev.IsAllowed = false;
                ev.Attacker.ClearInventory();
                var item = ev.Player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.CurrentItem = item;
                });
            }
        }
        public void OnJoin(VerifiedEventArgs ev) => ev.Player.Role.Set(RoleTypeId.Spectator);
        public void OnPickUpItem(PickingUpItemEventArgs ev) => ev.IsAllowed = false;
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
        */
    }
}
