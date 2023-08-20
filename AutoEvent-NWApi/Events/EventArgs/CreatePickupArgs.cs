using InventorySystem.Items.Pickups;
using InventorySystem.Items;

namespace AutoEvent.Events.EventArgs
{
    public class CreatePickupArgs
    {
        public CreatePickupArgs(PickupSyncInfo psi, ItemBase item, bool isAllowed = true)
        {
            Info = psi;
            Item = item;
            IsAllowed = isAllowed;
        }
        public PickupSyncInfo Info { get; }
        public ItemBase Item { get; }
        public bool IsAllowed { get; set; }
    }
}
