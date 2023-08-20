using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBloodDecal))]
    internal static class PlaceBlood
    {
        public static bool Prefix()
        {
            PlaceBloodArgs placeBloodEvent = new PlaceBloodArgs();
            Servers.OnPlaceBlood(placeBloodEvent);

            return placeBloodEvent.IsAllowed;
        }
    }
}
