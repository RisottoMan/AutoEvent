using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using Respawning;
using Respawning.Waves;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(WaveSpawner), nameof(WaveSpawner.SpawnWave))]
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
