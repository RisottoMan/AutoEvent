using AutoEvent.Events.EventArgs;
using InventorySystem.Configs;
using MEC;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Games.HideAndSeek
{
    public class EventHandler
    {
        public void OnPlayerDamage(PlayerDamageArgs ev)
        {
            //if (ev.DamageHandler.Type == DamageType.Falldown)
            //{
            //    ev.IsAllowed = false;
            //}

            if (ev.Attacker != null)
            {
                if (ev.Attacker.Items.First(r => r.ItemTypeId == ItemType.Jailbird) == true && ev.Target.Items.First(r => r.ItemTypeId == ItemType.Jailbird) == false)
                {
                    ev.IsAllowed = false;
                    ev.Attacker.ClearInventory();
                    var item = ev.Target.AddItem(ItemType.Jailbird);
                    Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Target.CurrentItem = item;
                    });
                }
            }
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
