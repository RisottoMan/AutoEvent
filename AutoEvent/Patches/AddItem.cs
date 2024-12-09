using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.AddItem), new [] { typeof(ItemType)})]
internal class AddItem
{
    public static bool Prefix(Player __instance, ItemType item, ref ItemBase __result)
    {
        ItemBase itemBase = __instance.ReferenceHub.inventory.ServerAddItem(item, 0);

        if (itemBase is Firearm firearm)
        {
            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(__instance.ReferenceHub, out var preferedAllAttachmets)
                && preferedAllAttachmets.TryGetValue(item, out var preferedAttachments))
            {
                firearm.ApplyAttachmentsCode(preferedAttachments, true);
            }
            
            if (firearm.TryGetModule(out MagazineModule magModule))
            {
                magModule.UserInv.ServerSetAmmo(magModule.AmmoType, magModule.AmmoMax);
                magModule.ServerInsertMagazine();
            }
            
            if (firearm.TryGetModule(out CylinderAmmoModule cylModule))
            {
                cylModule.ServerModifyAmmo(cylModule.AmmoMax);
            }
        }
        
        __result = itemBase;
        return false;
    }
}