using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using Respawning;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class TeamRespawn
    {
        public static bool Prefix()
        {
            TeamRespawnArgs teamRespawnEvent = new TeamRespawnArgs();
            Servers.OnTeamRespawn(teamRespawnEvent);

            return teamRespawnEvent.IsAllowed;
        }
    }
}
