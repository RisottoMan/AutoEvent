using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AutoEvent.API.Enums;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapGeneration.Distributors;
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
            return;
        
        if (Vector3.Distance(raycastHit.transform.gameObject.transform.position, _plugin.Button.transform.position) < 3)
        {
            ev.Player.ShowHitMarker(2f);
            _plugin.PrisonerDoors.GetComponent<JailerComponent>().ToggleDoor();
        }
    }

    public void OnDying(DyingEventArgs ev)
    {
        if (!ev.IsAllowed)
            return;

        if (_plugin.Deaths is null)
        {
            _plugin.Deaths = new Dictionary<Player, int>();
        }

        if (_plugin.Config.JailorLoadouts.Any(loadout => loadout.Roles.Any(role => role.Key == ev.Player.Role)))
            return;
        
        if (!_plugin.Deaths.ContainsKey(ev.Player))
        {
            _plugin.Deaths.Add(ev.Player, 1);
        }
        if (_plugin.Deaths[ev.Player] >= _plugin.Config.PrisonerLives)
        {
            ev.Player.ShowHint(_plugin.Translation.NoLivesRemaining, 4f);
            return;
        }

        int livesRemaining = _plugin.Config.PrisonerLives = _plugin.Deaths[ev.Player];
        ev.Player.ShowHint(_plugin.Translation.LivesRemaining.Replace("{lives}", livesRemaining.ToString()), 4f);
        ev.Player.GiveLoadout(_plugin.Config.PrisonerLoadouts);
        ev.Player.Position = _plugin.SpawnPoints.Where(r => r.name == "Spawnpoint").ToList().RandomItem().transform.position;
    }
    
    public void OnInteractingLocker(InteractingLockerEventArgs ev)
    {
        ev.IsAllowed = false;
        
        try
        {
            if (Vector3.Distance(ev.Player.Position, _plugin.MapInfo.Map.Position + new Vector3(13.1f, -12.23f, -12.14f)) < 2)
            {
                ev.Player.ClearInventory();
                ev.Player.GiveLoadout(_plugin.Config.WeaponLockerLoadouts,LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.DontClearDefaultItems);
            }
            else if (Vector3.Distance(ev.Player.Position, _plugin.MapInfo.Map.Position + new Vector3(17.855f, -12.43052f, -23.632f)) < 2)
            {
                ev.Player.GiveLoadout(_plugin.Config.AdrenalineLoadouts,LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons | LoadoutFlags.DontClearDefaultItems);
            }
            else if (Vector3.Distance(ev.Player.Position, _plugin.MapInfo.Map.Position + new Vector3(9f, -12.43052f, -21.78f)) < 2)
            {
                ev.Player.GiveLoadout(_plugin.Config.MedicalLoadouts,LoadoutFlags.IgnoreRole | LoadoutFlags.IgnoreGodMode | LoadoutFlags.IgnoreWeapons | LoadoutFlags.DontClearDefaultItems);
            }
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"An error has occured while processing locker events.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
    }
}