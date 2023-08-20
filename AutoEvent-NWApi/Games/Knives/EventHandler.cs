using AutoEvent.Events.EventArgs;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.Knives
{
    public class EventHandler
    {
        /*
        public void OnChargeJailbird(ChargingJailbirdEventArgs ev)
        {
            var item = (Jailbird)ev.Item;
            item.Base._chargeDuration = 0;
        }*/

        public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            /*
            if (ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.IsAllowed = false;
            }
            */
        }

        [PluginEvent(ServerEventType.PlayerDying)]
        public void OnJoin(PlayerDyingEvent ev)
        {
            ev.Player.ClearInventory();
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
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
