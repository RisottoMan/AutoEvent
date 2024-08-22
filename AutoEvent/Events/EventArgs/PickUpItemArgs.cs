using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class SearchPickUpItemArgs
    {
        public SearchPickUpItemArgs(Player player, ItemPickupBase pickup, SearchSession searchSession, SearchCompletor searchCompletor, float searchTime)
        {
            Player = player;
            Pickup = pickup;
            SearchSession = searchSession;
            SearchCompletor = searchCompletor;
            SearchTime = searchTime;
        }
        /// <summary>
        /// Gets or sets the SearchSession.
        /// </summary>
        public SearchSession SearchSession { get; set; }

        /// <summary>
        /// Gets or sets the SearchCompletor.
        /// </summary>
        public SearchCompletor SearchCompletor { get; set; }

        /// <summary>
        /// Gets or sets the Pickup search duration.
        /// </summary>
        public float SearchTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Pickup can be searched.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the Pickup that is being searched.
        /// </summary>
        public ItemPickupBase Pickup { get; }

        /// <summary>
        /// Gets the Player who's searching the Pickup.
        /// </summary>
        public Player Player { get; }
    }
}
