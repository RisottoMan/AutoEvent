using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Scp173TantrumAbility), nameof(Scp173TantrumAbility.ServerProcessCmd))]
    internal static class PlaceTantrum
    {
        public static bool Prefix()
        {
            PlaceTantrumArgs placeTantrumEvent = new PlaceTantrumArgs();
            Players.OnPlaceTantrum(placeTantrumEvent);

            return placeTantrumEvent.IsAllowed;
        }
    }
}
