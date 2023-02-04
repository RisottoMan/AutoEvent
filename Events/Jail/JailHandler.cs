using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Features.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AutoEvent.Events
{
    internal class JailHandler
    {
        public static void OnInteractLocker(InteractingLockerEventArgs ev)
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
                if (Vector3.Distance(ev.Player.Position, JailEvent.GameMap.gameObject.transform.position + new Vector3(17.855f, -12.43052f, -23.632f)) < 2)
                {
                    ev.Player.AddAhp(100, 100, 0);
                }
                else
                {
                    ev.Player.Health = ev.Player.MaxHealth;
                }
            }
        }
        public static void OnShootEvent(ShootingEventArgs ev)
        {
            if (!Physics.Raycast(ev.Player.ReferenceHub.PlayerCameraReference.transform.position, ev.Player.ReferenceHub.PlayerCameraReference.transform.forward, out RaycastHit raycastHit, 100f))
            {
                return;
            }
            if (Vector3.Distance(raycastHit.transform.gameObject.transform.position, JailEvent.Button.gameObject.transform.position) < 3)
            {
                if (JailEvent.isDoorsOpen)
                {
                    foreach (var obj in Object.FindObjectsOfType<PrimitiveObject>())
                    {
                        if (obj.name == "PrisonerDoor") obj.Position += new Vector3(2.2f, 0, 0);
                    }
                    JailEvent.isDoorsOpen = false;
                }
                else
                {
                    foreach (var obj in Object.FindObjectsOfType<PrimitiveObject>())
                    {
                        if (obj.name == "PrisonerDoor") obj.Position += new Vector3(-2.2f, 0, 0);
                    }
                    JailEvent.isDoorsOpen = true;
                }
            }
        }
        public static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
