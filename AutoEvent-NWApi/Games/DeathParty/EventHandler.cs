using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.DeathParty
{
    public class EventHandler
    {
        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Spectator);
        }

        public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            if (ev.Target != null && ev.DamageHandler is ExplosionDamageHandler)
            {
                if (Plugin.Stage != 5)
                {
                    ev.IsAllowed = false;
                    ev.Target.Damage(10, "Grenade");
                }
                else
                {
                    ev.Target.Damage(100, "Grenade");
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}