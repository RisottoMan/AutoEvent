using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using PluginAPI.Core;
using PluginAPI.Core.Items;
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
            
            Firearm firearm = (Firearm)itemBase;
            if (firearm != null)
            {
                /*
                AttachmentIdentifier[] value;
                if (identifiers != null)
                {
                    //firearm.AddAttachment(identifiers);
                    foreach (AttachmentIdentifier identifier in identifiers)
                    {
                        AddAttachment(identifier);
                    }
                }
                else if (Preferences != null && Preferences.TryGetValue(itemType.GetFirearmType(), out value))
                {
                    firearm.ApplyAttachmentsCode(value.GetAttachmentsCode(), reValidate: true);
                }
                */
                FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;
                if (firearm.Attachments.Any((Attachment a) => a.Name == AttachmentName.Flashlight))
                {
                    firearmStatusFlags |= FirearmStatusFlags.FlashlightEnabled;
                }

                firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, firearmStatusFlags, firearm.GetCurrentAttachmentsCode());
            }

            return false;
        }
    }
}
