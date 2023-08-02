using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem;
using System;
using UnityEngine;
using InventorySystem.Configs;

namespace AutoEvent.Patch
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup), new Type[] { typeof(ItemBase), typeof(PickupSyncInfo), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Action<ItemPickupBase>) })]
    internal class CreatePickupPatch
    {
        internal static void Prefix(ItemBase item, PickupSyncInfo psi, Vector3 position, Quaternion rotation, ref bool spawn, Action<ItemPickupBase> setupMethod)
        {
            if (AutoEvent.ActiveEvent == null) return;

            if (InventoryLimits.StandardAmmoLimits.ContainsKey(item.ItemTypeId))
            {
                spawn = false;
            }
        }
    }
}
