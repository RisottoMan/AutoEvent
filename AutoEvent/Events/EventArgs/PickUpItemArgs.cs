using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class PickUpItemArgs
    {
        public PickUpItemArgs(Player player, ItemBase item, ItemPickupBase pickup, bool isAllowed = true)
        {
            IsAllowed = isAllowed;
            Pickup = pickup;
            Item = item;
            Player = player;
        }
        public bool IsAllowed { get; set; }
        public ItemBase Item { get; set; }
        public ItemPickupBase Pickup { get; set; }
        public Player Player { get; set; }
    }
}
