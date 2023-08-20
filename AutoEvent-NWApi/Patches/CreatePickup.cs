using HarmonyLib;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem;
using System;
using UnityEngine;
using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup), new Type[] { typeof(ItemBase), typeof(PickupSyncInfo), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Action<ItemPickupBase>) })]
    internal class CreatePickup
    {
        internal static void Prefix(ItemBase item, PickupSyncInfo psi, Vector3 position, Quaternion rotation, ref bool spawn, Action<ItemPickupBase> setupMethod)
        {
            CreatePickupArgs createPickupEvent = new CreatePickupArgs(psi, item, spawn);
            Servers.OnCreatePickup(createPickupEvent);

            spawn = createPickupEvent.IsAllowed;
        }
    }
}
