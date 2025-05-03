using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;

namespace AutoEvent.Events.EventArgs
{
    public class Scp018UpdateArgs
    {
        public Scp018UpdateArgs(Scp018Projectile proj)
        {
            Player = Player.Get(proj.PreviousOwner.Hub);
            Projectile = proj;
        }
        public Player Player { get; }
        public Scp018Projectile Projectile { get; }
    }
}
