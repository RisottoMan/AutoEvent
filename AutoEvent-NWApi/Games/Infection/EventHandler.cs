using AutoEvent.Events.EventArgs;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Linq;

namespace AutoEvent.Games.Infection
{
    public class EventHandler
    {
        public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            if (ev.DamageType == DeathTranslations.Falldown.Id)
            {
                ev.IsAllowed = false;
            }

            if (ev.Attacker != null)
            {
                if (ev.Attacker.Role == RoleTypeId.Scp0492)
                {
                    ev.Target.SetRole(RoleTypeId.Scp0492);
                    ev.Attacker.ReceiveHitMarker();
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scp0492) > 0)
            {
                Extensions.SetRole(ev.Player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }
            else
            {
                Extensions.SetRole(ev.Player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                ev.Player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            }
        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnDeath(PlayerDeathEvent ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                Extensions.SetRole(ev.Player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomPosition.GetSpawnPosition(Plugin.GameMap);
            });
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
    }
}
