using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class Scp018BounceArgs
    {
        public Scp018BounceArgs(Player player, ItemPickupBase pickup, ThrownProjectile proj, bool isBounced, bool isAllowed = true)
        {
            Player = player;
            Pickup = pickup;
            Projectile = proj;
            IsBounced = isBounced;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public ItemPickupBase Pickup { get; }
        public ThrownProjectile Projectile { get; }
        public bool IsBounced { get; }
        public bool IsAllowed { get; set; }
    }
}
