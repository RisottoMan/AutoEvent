using Exiled.API.Features;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent.Patch
{
    internal class CleanupPatch
    {
        [HarmonyPatch(typeof(InventoryExtensions), "ServerCreatePickup")]
        static class CreatePickup
        {
            internal static void Prefix(Inventory inv, ItemBase item, PickupSyncInfo psi, ref bool spawn)
            {
                if (AutoEvent.ActiveEvent == null) return;
                spawn = false;
            }
        }
    }
}
