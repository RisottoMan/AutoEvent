using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Features.Objects;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace AutoEvent.Events.HideAndSeek
{
    public class EventHandler
    {
        public void OnJoin(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }
        public void OnHurt(HurtingEventArgs ev)
        {
            if (ev.Player != null && ev.DamageHandler.Type == DamageType.Falldown)
            {
                ev.IsAllowed = false;
            }

            if (ev.Attacker.HasItem(ItemType.Jailbird) == true && ev.Player.HasItem(ItemType.Jailbird) == false)
            {
                ev.IsAllowed = false;
                ev.Attacker.ClearInventory();
                var item = ev.Player.AddItem(ItemType.Jailbird);
                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.CurrentItem = item;
                });
            }
        }
        public void OnShooting(ShootingEventArgs ev)
        {
            /*
            if (!Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.transform.position, ev.Player.ReferenceHub.PlayerCameraReference.transform.forward, out RaycastHit raycastHit, 100f))
            {
                return;
            }
            if (Vector3.Distance(raycastHit.transform.gameObject.transform.position, _plugin.Button.gameObject.transform.position) < 3)
            {
                if (_plugin.isDoorsOpen)
                {
                    foreach (var obj in Object.FindObjectsOfType<PrimitiveObject>())
                    {
                        if (obj.name == "PrisonerDoor") obj.Position += new Vector3(2.2f, 0, 0);
                    }
                    _plugin.isDoorsOpen = false;
                }
                else
                {
                    foreach (var obj in Object.FindObjectsOfType<PrimitiveObject>())
                    {
                        if (obj.name == "PrisonerDoor") obj.Position += new Vector3(-2.2f, 0, 0);
                    }
                    _plugin.isDoorsOpen = true;
                }
            }
            */
        }
        public void OnPickUpItem(PickingUpItemEventArgs ev) => ev.IsAllowed = false;
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
        public void OnSpawnRagdoll(SpawningRagdollEventArgs ev) => ev.IsAllowed = false;
        public void OnPlaceBlood(PlacingBloodEventArgs ev) => ev.IsAllowed = false;
        public void OnDropItem(DroppingItemEventArgs ev) => ev.IsAllowed = false;
        public void OnDropAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
    }
}
