using AutoEvent.Events.EventArgs;
using MapGeneration.Distributors;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace AutoEvent.Games.Jail
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
        }

        [PluginEvent(ServerEventType.PlayerInteractLocker)]
        public void OnInteractLocker(PlayerInteractLockerEvent ev)
        {
            // ev.IsAllowed = false;
            if (ev.Locker.StructureType == StructureType.StandardLocker)
            {
                ev.Player.ClearInventory();
                ev.Player.AddItem(ItemType.GunE11SR);
                ev.Player.AddItem(ItemType.GunCOM18);
            }

            if (ev.Locker.StructureType == StructureType.SmallWallCabinet)
            {
                if (Vector3.Distance(ev.Player.Position, _plugin.GameMap.gameObject.transform.position + new Vector3(17.855f, -12.43052f, -23.632f)) < 2)
                {
                    Extensions.SetPlayerAhp(ev.Player, 100, 100, 0);
                    ev.Player.ArtificialHealth = 100;
                }
                else
                {
                    ev.Player.Health = ev.Player.MaxHealth;
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerShotWeapon)]
        public void PlayerShoot(PlayerShotWeaponEvent ev)
        {
            if (!Physics.Raycast(ev.Player.Camera.position, ev.Player.Camera.forward, out RaycastHit raycastHit, 100f))
            {
                return;
            }

            if (Vector3.Distance(raycastHit.transform.gameObject.transform.position, _plugin.Button.transform.position) < 3)
            {
                _plugin.PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
            }
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    }
}
