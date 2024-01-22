using AutoEvent.Events.EventArgs;
using Exiled.Events.EventArgs.Player;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace AutoEvent.Games.Snowball
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
            if (ev.AttackerHandler is Scp018DamageHandler ballDamagehandler)
            {
                ballDamagehandler.Damage = 50;
            }
        }

        public void OnScp018Bounce(Scp018BounceArgs ev)
        {
            foreach(Player player in Player.GetPlayers())
            {
                if (ev.Player.Equals(player))
                    continue;

                if (Vector3.Distance(ev.Pickup.Position, player.Position) < 5)
                {
                    player.Damage(50, $"<color=red>{player.Nickname} kill you!</color>");
                    ev.Pickup.DestroySelf();
                }
            }

            if (ev.IsBounced is true)
            {
                ev.Pickup.DestroySelf();
            }
        }

        [PluginEvent(ServerEventType.PlayerThrowProjectile)]
        public void OnPlayerThrowProjectile(PlayerThrowProjectileEvent ev)
        {
            //NetworkServer.UnSpawn(ev.Item.gameObject);
            //ev.Item.gameObject.transform.localScale *= 5;
            //NetworkServer.Spawn(ev.Item.gameObject);
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
