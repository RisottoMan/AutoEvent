using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using Decals;
using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Events;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(PlaceBloodEvent), nameof(StandardHitregBase.PlaceBloodDecal))]
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
