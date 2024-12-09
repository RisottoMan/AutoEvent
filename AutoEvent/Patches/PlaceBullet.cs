using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(ImpactEffectsModule), nameof(ImpactEffectsModule.ServerSendImpactDecal))]
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
