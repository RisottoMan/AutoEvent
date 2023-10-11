using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using Mirror;
using PlayerRoles.FirstPersonControl.NetworkMessages;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
    internal static class PlayerNoclip
    {
        public static bool Prefix(NetworkConnection sender)
        {
            if (!ReferenceHub.TryGetHubNetID(sender.identity.netId, out var hub)) return false;

            PlayerNoclipArgs playerNoclipEvent = new PlayerNoclipArgs(hub);
            Players.OnPlayerNoclip(playerNoclipEvent);

            return playerNoclipEvent.IsAllowed;
        }
    }
}
