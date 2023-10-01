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
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

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
                    // ev.Target.SetRole(RoleTypeId.Scp0492);
                    ev.Target.GiveLoadout(_plugin.Config.ZombieLoadouts);
                    ev.Attacker.ReceiveHitMarker(1f);
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        public void OnJoin(PlayerJoinedEvent ev)
        {
            if (Player.GetPlayers().Count(r => r.Role == RoleTypeId.Scp0492) > 0)
            {
                //Extensions.SetRole(ev.Player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                ev.Player.GiveLoadout(_plugin.Config.ZombieLoadouts);
                ev.Player.Position = RandomPosition.GetSpawnPosition(_plugin.MapInfo.Map);
            }
            else
            {
                ev.Player.GiveLoadout(_plugin.Config.PlayerLoadouts);
                // Extensions.SetRole(ev.Player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                ev.Player.Position = RandomPosition.GetSpawnPosition(_plugin.MapInfo.Map);
            }
        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        public void OnDeath(PlayerDeathEvent ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                ev.Player.GiveLoadout(_plugin.Config.ZombieLoadouts);
                //Extensions.SetRole(ev.Player, RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
                ev.Player.Position = RandomPosition.GetSpawnPosition(_plugin.MapInfo.Map);
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
