using System;
using AutoEvent.Events.EventArgs;
using MapGeneration.Distributors;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;
using AutoEvent;
using AutoEvent.API.Enums;

namespace AutoEvent.Games.Jail
{
    public class EventHandler
    {
        Plugin _plugin;
        public EventHandler(Plugin plugin)
        {
            _plugin = plugin;
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


        public void OnLockerInteract(LockerInteractArgs ev)
        {
            ev.IsAllowed = false;
            try
            {

                if (ev.LockerType == StructureType.StandardLocker)
                {
                    try
                    {

                        foreach (var userInventoryItem in ev.Player.ReferenceHub.inventory.UserInventory.Items)
                        {
                            var i = userInventoryItem;
                            if (i.Value.ItemTypeId.IsWeapon())
                            {
                                ev.Player.RemoveItem(i.Value);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugLogger.LogDebug($"{e}");
                    }

                    ev.Player.GiveLoadout(_plugin.Config.WeaponLockerLoadouts,
                        LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode);
                }

                if (ev.Locker.StructureType == StructureType.SmallWallCabinet)
                {
                    if (Vector3.Distance(ev.Player.Position,
                            _plugin.MapInfo.Map.gameObject.transform.position +
                            new Vector3(17.855f, -12.43052f, -23.632f)) < 2)
                    {
                        ev.Player.GiveLoadout(_plugin.Config.MedicalLoadouts,
                            LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
                        //Extensions.SetPlayerAhp(ev.Player, 100, 100, 0);
                        //ev.Player.ArtificialHealth = 100;
                    }
                    else
                    {
                        ev.Player.GiveLoadout(_plugin.Config.AdrenalineLoadouts,
                            LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons);
                        // ev.Player.Health = ev.Player.MaxHealth;
                    }
                }
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug($"An error has occured while processing locker events.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}");
            }
        }

        public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    }
}
