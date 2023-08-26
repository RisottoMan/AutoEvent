using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using PlayerStatsSystem;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal class CassieScp
    {
        public static bool Prefix(ReferenceHub scp, DamageHandlerBase hit)
        {
            CassieScpArgs cassieScpEvent = new CassieScpArgs();
            Servers.OnCassieScp(cassieScpEvent);

            return cassieScpEvent.IsAllowed;
        }
    }
}
