using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MapGeneration.Distributors;
using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Events.Jail
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }
        public void OnInteractLocker(InteractingLockerEventArgs ev)
        {
            ev.IsAllowed = false;
            if (ev.Locker.StructureType == StructureType.LargeGunLocker)
            {
                ev.Player.ResetInventory(new List<ItemType>
                {
                    ItemType.GunE11SR,
                    ItemType.GunCOM18
                });
            }
            if (ev.Locker.StructureType == StructureType.SmallWallCabinet)
            {
                if (Vector3.Distance(ev.Player.Position, _plugin.GameMap.gameObject.transform.position + new Vector3(17.855f, -12.43052f, -23.632f)) < 2)
                {
                    ev.Player.AddAhp(100, 100, 0);
                }
                else
                {
                    ev.Player.Health = ev.Player.MaxHealth;
                }
            }
        }
        public void OnShootEvent(ShootingEventArgs ev)
        {
            if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit, 100f))
            {
                return;
            }

            if (Vector3.Distance(raycastHit.transform.gameObject.transform.position, _plugin.Button.transform.position) < 3)
            {
                foreach (var door in _plugin.JailerDoors)
                {
                    door.GetComponent<JailerComponent>().ToggleDoor();
                }
            }
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) => ev.IsAllowed = false;
    }
}
