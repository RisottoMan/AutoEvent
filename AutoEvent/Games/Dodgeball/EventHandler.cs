using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PlayerStatsSystem;

namespace AutoEvent.Games.Dodgeball
{
    public class EventHandler
    {
        private Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }
        public void OnScp018Bounce(Scp018BounceArgs ev)
        {
            ev.Pickup.DestroySelf();
        }
        public void OnDamage(PlayerDamageArgs ev)
        {
            if (ev.AttackerHandler is Scp018DamageHandler ballDamagehandler)
            {
                ballDamagehandler.Damage = 50;
            }
        }
        [PluginEvent(ServerEventType.PlayerThrowProjectile)]
        public void OnPlayerThrowProjectile(PlayerThrowProjectileEvent ev)
        {
            //_plugin.BallObjects.Add(ev.Item.PickupDropModel.gameObject);
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            ev.Player.SetRole(RoleTypeId.Spectator);
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
