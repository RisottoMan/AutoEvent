using AutoEvent.Events.EventArgs;
using AutoEvent.Interfaces;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Linq;

namespace AutoEvent.Games.MusicalChairs
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        public void OnDamage(PlayerDamageArgs ev)
        {
            if (ev.AttackerHandler is ExplosionDamageHandler explosionDamageHandler)
            {
                explosionDamageHandler.Damage = 0;
            }
        }

        public void OnUsingStamina(UsingStaminaArgs ev)
        {
            ev.IsAllowed = false;
        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnPlayerDeath(PlayerDeathEvent ev)
        {
            _plugin.Platforms = Functions.RearrangePlatforms(
                Player.GetPlayers().Count(r => r.IsAlive), 
                _plugin.Platforms,
                _plugin.MapInfo.Position);
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
        public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
