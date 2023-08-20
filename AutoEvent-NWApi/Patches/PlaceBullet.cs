using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
    internal static class PlaceBullet
    {
        public static bool Prefix()
        {
            PlaceBulletArgs placeBulletEvent = new PlaceBulletArgs();
            Servers.OnPlaceBullet(placeBulletEvent);

            return placeBulletEvent.IsAllowed;
        }
    }
}
