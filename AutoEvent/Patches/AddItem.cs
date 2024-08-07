using Exiled.API.Enums;
using Exiled.API.Extensions;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.AddItem))]
    internal class AddItem
    {
        public static bool Prefix(Player __instance, ItemType item, ref ItemBase __result)
        {
            ItemBase itemBase = __instance.ReferenceHub.inventory.ServerAddItem(item, 0);

            if (itemBase is Firearm firearm)
            {
                FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;

                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(__instance.ReferenceHub, out var preferedAllAttachmets)
                    && preferedAllAttachmets.TryGetValue(item, out var preferedAttachments))
                {
                    firearmStatusFlags |= (FirearmStatusFlags)preferedAttachments;
                }

                if (firearm.Attachments.Any((Attachment a) => a.Name == AttachmentName.Flashlight))
                {
                    firearmStatusFlags |= FirearmStatusFlags.FlashlightEnabled;
                }

                firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, firearmStatusFlags, firearm.GetCurrentAttachmentsCode());
            }

            __result = itemBase;
            return false;
        }
    }
}
