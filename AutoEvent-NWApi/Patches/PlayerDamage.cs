using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using PlayerStatsSystem;
using PluginAPI.Core;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
    internal static class PlayerDamage
    {
        private static bool Prefix(PlayerStats __instance, DamageHandlerBase handler) // bug lava
        {
            Log.Info("Player Damage");
            Player player = Player.Get(__instance._hub);
            PlayerDamageArgs ev = new(player, handler);
            Players.OnPlayerDamage(ev);

            return ev.IsAllowed;
        }
    }
}
