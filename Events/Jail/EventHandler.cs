using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Features.Objects;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
            if (ev.Locker.StructureType == MapGeneration.Distributors.StructureType.LargeGunLocker)
            {
                ev.Player.ResetInventory(new List<ItemType>
                {
                    ItemType.GunE11SR,
                    ItemType.GunCOM18
                });
            }
            if (ev.Locker.StructureType == MapGeneration.Distributors.StructureType.SmallWallCabinet)
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
        }
        public void OnTeamRespawn(RespawningTeamEventArgs ev) { ev.IsAllowed = false; }
    }
}
