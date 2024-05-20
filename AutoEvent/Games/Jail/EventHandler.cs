using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.Events.EventArgs;
using MapGeneration.Distributors;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;
using AutoEvent.API.Enums;
using InventorySystem.Items;
using PluginAPI.Core;
using Utils.NonAllocLINQ;

namespace AutoEvent.Games.Jail;

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
            BypassLevel bypassLevel = BypassLevel.None;
            if (ev.Player.Items.Any(x => x.ItemTypeId is ItemType.KeycardContainmentEngineer))
                bypassLevel = BypassLevel.ContainmentEngineer;
            if(ev.Player.Items.Any(x => x.ItemTypeId is ItemType.KeycardO5))
                bypassLevel = BypassLevel.O5;
            if (ev.Player.IsBypassEnabled)
                bypassLevel = BypassLevel.BypassMode;
            
            DebugLogger.LogDebug("Passing on from raycast..");
            if (!_plugin.JailLockdownSystem.ToggleLockdown(bypassLevel))
            {
                ev.Player.ReceiveHint(_plugin.Translation.LockdownOnCooldown.Replace("{cooldown}", _plugin.JailLockdownSystem.LockDownCooldown.ToString()), 5f);
                return;
            }
            ev.Player.ReceiveHitMarker(2f);
            //_plugin.PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
        }
    }

    public void OnPlayerDying(PlayerDyingArgs ev)
    {
        DebugLogger.LogDebug("Player Died.");
        if (!ev.IsAllowed)
            return;

        if (_plugin.Deaths is null)
        {
            _plugin.Deaths = new Dictionary<Player, int>();
        }
        
        if (_plugin.Config.JailorLoadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ev.Target.Role)))
        {
            DebugLogger.LogDebug("Player was jailor. Skipping.");
            return;
        }
        if (!_plugin.Deaths.ContainsKey(ev.Target))
        {
            DebugLogger.LogDebug("Player has one death.");
            _plugin.Deaths.Add(ev.Target, 1);
        }
        if (_plugin.Deaths[ev.Target] >= _plugin.Config.PrisonerLives)
        {
            ev.Target.ReceiveHint(_plugin.Translation.NoLivesRemaining, 4f);
            DebugLogger.LogDebug("Player has no lives left.");
            return;
        }
        DebugLogger.LogDebug("Respawning Player.");

        int livesRemaining = _plugin.Config.PrisonerLives = _plugin.Deaths[ev.Target];
        ev.Target.ReceiveHint(_plugin.Translation.LivesRemaining.Replace("{lives}", livesRemaining.ToString()), 4f);
        ev.Target.GiveLoadout(_plugin.Config.PrisonerLoadouts);
        try
        {
            ev.Target.Position = JailRandom.GetRandomPosition(_plugin.MapInfo.Map, false);
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Could not set player position.");
        }

    }
    public void OnLockerInteract(LockerInteractArgs ev)
    {
        ev.IsAllowed = false;
        try
        {

            if (ev.LockerType == StructureType.LargeGunLocker)
            {
                try
                {
                    List<ItemBase> itemsToRemove = new List<ItemBase>();
                    foreach (var userInventoryItem in ev.Player.ReferenceHub.inventory.UserInventory.Items)
                    {
                        if (userInventoryItem.Value.ItemTypeId.IsWeapon())
                        {
                            itemsToRemove.Add(userInventoryItem.Value);
                        }
                    }

                    foreach (var item in itemsToRemove)
                    {
                        ev.Player.RemoveItem(item);
                    }
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"{e}");
                }

                ev.Player.GiveLoadout(_plugin.Config.WeaponLockerLoadouts,
                    LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.DontClearDefaultItems);
            }

            if (ev.Locker.StructureType == StructureType.SmallWallCabinet)
            {
                if (Vector3.Distance(ev.Player.Position,
                        _plugin.MapInfo.Map.gameObject.transform.position +
                        new Vector3(17.855f, -12.43052f, -23.632f)) < 2)
                {
                    ev.Player.GiveLoadout(_plugin.Config.AdrenalineLoadouts,
                        LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons | LoadoutFlags.DontClearDefaultItems);
                }
                else
                {
                    ev.Player.GiveLoadout(_plugin.Config.MedicalLoadouts,
                        LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons | LoadoutFlags.DontClearDefaultItems);
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