using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments.Components;
using PluginAPI.Core;
using PluginAPI.Core.Items;
using System.Linq;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(Player), nameof(Player.AddItem))]
    internal static class AddItem
    {
        public static bool Prefix(Player __instance, ItemType item)
        {
            /*
            Item item = Item.Get(__instance.ReferenceHub.inventory.ServerAddItem(item, 0));
            Firearm firearm = Firearm(item);
            if (firearm != null)
            {
                AttachmentIdentifier[] value;
                if (identifiers != null)
                {
                    firearm.AddAttachment(identifiers);
                }
                else if (Preferences != null && Preferences.TryGetValue(itemType.GetFirearmType(), out value))
                {
                    firearm.Base.ApplyAttachmentsCode(value.GetAttachmentsCode(), reValidate: true);
                }

                FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;
                if (firearm.Attachments.Any((Attachment a) => a.Name == AttachmentName.Flashlight))
                {
                    firearmStatusFlags |= FirearmStatusFlags.FlashlightEnabled;
                }

                firearm.Base.Status = new FirearmStatus(firearm.MaxAmmo, firearmStatusFlags, firearm.Base.GetCurrentAttachmentsCode());
            }
            */
            return false;
        }
    }
}
