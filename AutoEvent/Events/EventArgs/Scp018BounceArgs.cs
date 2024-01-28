using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class Scp018BounceArgs
    {
        public Scp018BounceArgs(ReferenceHub hub, ItemPickupBase pickup, ThrownProjectile proj, bool isAllowed = true)
        {
            Player = Player.Get(hub);
            Pickup = pickup;
            Projectile = proj;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public ItemPickupBase Pickup { get; }
        public ThrownProjectile Projectile { get; }
        public bool IsAllowed { get; set; }
    }
}
