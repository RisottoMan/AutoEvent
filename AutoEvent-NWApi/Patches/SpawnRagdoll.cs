using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using PlayerRoles.Ragdolls;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    internal static class SpawnRagdoll
    {
        public static bool Prefix()
        {
            SpawnRagdollArgs spawnRagdollEvent = new SpawnRagdollArgs();
            Servers.OnSpawnRagdoll(spawnRagdollEvent);

            return spawnRagdollEvent.IsAllowed;
        }
    }
}
