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
        public static event Action<CassieScpArgs> CassieScp;
        public static event Action<Scp018CollisionArgs> Scp018Collision;
        public static event Action<Scp018UpdateArgs> Scp018Update;
        public static void OnPlaceBlood(PlaceBloodArgs ev) => PlaceBlood?.Invoke(ev);
        public static void OnPlaceBullet(PlaceBulletArgs ev) => PlaceBullet?.Invoke(ev);
        public static void OnSpawnRagdoll(SpawnRagdollArgs ev) => SpawnRagdoll?.Invoke(ev);
        public static void OnTeamRespawn(TeamRespawnArgs ev) => TeamRespawn?.Invoke(ev);
        public static void OnCassieScp(CassieScpArgs ev) => CassieScp?.Invoke(ev);
        public static void OnScp018Collision(Scp018CollisionArgs ev) => Scp018Collision?.Invoke(ev);
        public static void OnScp018Update(Scp018UpdateArgs ev) => Scp018Update?.Invoke(ev);
    }
}
