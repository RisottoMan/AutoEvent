using InventorySystem.Items.ThrowableProjectiles;
using Exiled.API.Features;

namespace AutoEvent.Events.EventArgs
{
    public class Scp018CollisionArgs
    {
        public Scp018CollisionArgs(Scp018Projectile proj)
        {
            Player = Player.Get(proj.PreviousOwner.Hub);
            Projectile = proj;
        }
        public Player Player { get; }
        public Scp018Projectile Projectile { get; }
    }
}
