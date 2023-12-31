using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using PlayerRoles.FirstPersonControl;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdateGrounded))]
    internal static class PlayerJump
    {
        public static bool Prefix(FpcMotor __instance)
        {
            PlayerJumpArgs playerJumpArgs = new PlayerJumpArgs(__instance.Hub);
            Players.OnPlayerJump(playerJumpArgs);

            return playerJumpArgs.IsAllowed;
        }
    }
}
