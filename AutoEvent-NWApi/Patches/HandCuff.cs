using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using InventorySystem.Disarming;
using Mirror;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal class HandCuff
    {
        public static bool Prefix(NetworkConnection conn, DisarmMessage msg)
        {
            HandCuffArgs handCuffEvent = new HandCuffArgs();
            Players.OnHandCuff(handCuffEvent);

            return handCuffEvent.IsAllowed;
        }
    }
}
