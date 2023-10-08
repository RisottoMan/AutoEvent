using AutoEvent.Events.EventArgs;
using System;

namespace AutoEvent.Events.Handlers
{
    public static class Servers
    {
        public static event Action<PlaceBloodArgs> PlaceBlood;
        public static event Action<PlaceBulletArgs> PlaceBullet;
        public static event Action<SpawnRagdollArgs> SpawnRagdoll;
        public static event Action<TeamRespawnArgs> TeamRespawn;
        public static event Action<RemoteAdminArgs> RemoteAdmin;
        public static event Action<CassieScpArgs> CassieScp;
        public static void OnPlaceBlood(PlaceBloodArgs ev) => PlaceBlood?.Invoke(ev);
        public static void OnPlaceBullet(PlaceBulletArgs ev) => PlaceBullet?.Invoke(ev);
        public static void OnSpawnRagdoll(SpawnRagdollArgs ev) => SpawnRagdoll?.Invoke(ev);
        public static void OnTeamRespawn(TeamRespawnArgs ev) => TeamRespawn?.Invoke(ev);
        public static void OnRemoteAdmin(RemoteAdminArgs ev) => RemoteAdmin?.Invoke(ev);
        public static void OnCassieScp(CassieScpArgs ev) => CassieScp?.Invoke(ev);
    }
}
