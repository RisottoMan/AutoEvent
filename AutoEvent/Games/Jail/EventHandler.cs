using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.API.Enums;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Utils.NonAllocLINQ;

namespace AutoEvent.Games.Jail;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnShooting(ShootingEventArgs ev)
    {
        if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit, 100f, 1 << 0))
        {
            return;
        }
        
        if (Vector3.Distance(raycastHit.transform.gameObject.transform.position, _plugin.Button.transform.position) < 3)
        {
            BypassLevel bypassLevel = BypassLevel.None;
            if (ev.Player.Items.Any(x => x.Type is ItemType.KeycardContainmentEngineer))
                bypassLevel = BypassLevel.ContainmentEngineer;
            if(ev.Player.Items.Any(x => x.Type is ItemType.KeycardO5))
                bypassLevel = BypassLevel.O5;
            if (ev.Player.IsBypassModeEnabled)
                bypassLevel = BypassLevel.BypassMode;
            
            DebugLogger.LogDebug("Passing on from raycast..");
            if (!_plugin.JailLockdownSystem.ToggleLockdown(bypassLevel))
            {
                ev.Player.ShowHint(_plugin.Translation.LockdownOnCooldown.Replace("{cooldown}", _plugin.JailLockdownSystem.LockDownCooldown.ToString()), 5f);
                return;
            }
            ev.Player.ShowHitMarker(2f);
            _plugin.PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
        }
    }

    public void OnDying(DyingEventArgs ev)
    {
        DebugLogger.LogDebug("Player Died.");
        if (!ev.IsAllowed)
            return;

        if (_plugin.Deaths is null)
        {
            _plugin.Deaths = new Dictionary<Player, int>();
        }
        
        if (_plugin.Config.JailorLoadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ev.Player.Role)))
        {
            DebugLogger.LogDebug("Player was jailor. Skipping.");
            return;
        }
        if (!_plugin.Deaths.ContainsKey(ev.Player))
        {
            DebugLogger.LogDebug("Player has one death.");
            _plugin.Deaths.Add(ev.Player, 1);
        }
        if (_plugin.Deaths[ev.Player] >= _plugin.Config.PrisonerLives)
        {
            ev.Player.ShowHint(_plugin.Translation.NoLivesRemaining, 4f);
            DebugLogger.LogDebug("Player has no lives left.");
            return;
        }
        DebugLogger.LogDebug("Respawning Player.");

        int livesRemaining = _plugin.Config.PrisonerLives = _plugin.Deaths[ev.Player];
        ev.Player.ShowHint(_plugin.Translation.LivesRemaining.Replace("{lives}", livesRemaining.ToString()), 4f);
        ev.Player.GiveLoadout(_plugin.Config.PrisonerLoadouts);
        try
        {
            ev.Player.Position = JailRandom.GetRandomPosition(_plugin.MapInfo.Map, false);
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Could not set player position.");
        }

    }
    public void OnInteractingLocker(InteractingLockerEventArgs ev)
    {
        ev.IsAllowed = false;
        try
        {
            if (ev.InteractingLocker.Type == LockerType.LargeGun)
            {
                try
                {
                    foreach (var item in ev.Player.Items)
                    {
                        if (item.IsWeapon())
                        {
                            ev.Player.RemoveItem(item);
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"{e}");
                }

                ev.Player.GiveLoadout(_plugin.Config.WeaponLockerLoadouts,
                    LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.DontClearDefaultItems);
            }

            if (ev.InteractingLocker.Type == LockerType.Adrenaline) //SmallWallCabinet
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
}