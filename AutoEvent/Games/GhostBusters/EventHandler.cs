// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EventHandlers.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 1:51 AM
//    Created Date:     10/28/2023 1:51 AM
// -----------------------------------------

using AutoEvent.Events.EventArgs;
using InventorySystem.Items.Firearms;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent.Games.GhostBusters;

public class EventHandler
{
    private Plugin _plugin { get; set; }
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    [PluginEvent(ServerEventType.PlayerLeft)]
    public void OnPlayerLeft(PlayerLeftEvent ev)
    {
        if (_plugin.Hunters.Contains(ev.Player))
        {
            _plugin.Hunters.Remove(ev.Player);
        }
    }

    [PluginEvent(ServerEventType.PlayerShotWeapon)]
    public void OnShotWeapon(PlayerShotWeaponEvent ev)
    {
        ev.Firearm._status = new FirearmStatus(byte.MaxValue,
            FirearmStatusFlags.Chambered | FirearmStatusFlags.MagazineInserted | FirearmStatusFlags.Cocked,
            ev.Firearm._status.Attachments);
    }

    [PluginEvent(ServerEventType.PlayerDeath)]
    public void OnPlayerDeath(PlayerDeathEvent ev)
    {
        if (_plugin.Hunters.Contains(ev.Player))
        {
            _plugin.Hunters.Remove(ev.Player);
        }
    }
    
    [PluginEvent(ServerEventType.PlayerDamage)]
    public bool OnPlayerDamage(PlayerDamageEvent ev)
    {
        if (ev.Target == ev.Player)
        {
            return false;
        }
        bool targetIsGhost = _plugin.IsGhost(ev.Target);
        bool attackerIsGhost = _plugin.IsGhost(ev.Player);
        if (ev.DamageHandler is ExplosionDamageHandler explosion)
        {
            if (targetIsGhost && attackerIsGhost)
                return false;

            if (!targetIsGhost && !attackerIsGhost)
                return false;
                
            if (!targetIsGhost && attackerIsGhost)
            {
                explosion.Damage = _plugin.Damages[ItemType.GrenadeHE];
                return true;
            }
            
            return true;
        }

        if (ev.DamageHandler is DisruptorDamageHandler disruptor)
        {
            if (targetIsGhost && attackerIsGhost)
                return false;
            
            if (!targetIsGhost && !attackerIsGhost)
                return false;
            
            if (targetIsGhost && !attackerIsGhost)
            {
                disruptor.Damage = _plugin.Damages[ItemType.ParticleDisruptor];
                return true;
            }
             
            return true;
        }

        /*if (ev.DamageHandler is Scp018DamageHandler ball)
        {
            if (targetIsGhost && attackerIsGhost)
                return true;
            
            if (!targetIsGhost && !attackerIsGhost)
                return true;
            
            if (targetIsGhost && !attackerIsGhost)
            {
                disruptor.Damage = _plugin.Damages[ItemType.ParticleDisruptor];
                return true;
            }
             
            return true;
        }*/
        if (ev.DamageHandler is MicroHidDamageHandler micro)
        {
            if (targetIsGhost && attackerIsGhost)
                return false;
            
            if (!targetIsGhost && !attackerIsGhost)
                return false;

            micro.Damage = _plugin.Damages[ItemType.MicroHID];
            return true;
        }

        if (ev.DamageHandler is JailbirdDamageHandler jailbird)
        {
            if (targetIsGhost && attackerIsGhost)
                return false;
            
            if (!targetIsGhost && !attackerIsGhost)
                return false; 

            jailbird.Damage = _plugin.Damages[ItemType.Jailbird];
            return true;
        }


        return true;
    }
    public void OnTeamRespawn(TeamRespawnArgs ev) => ev.IsAllowed = false;
    public void OnSpawnRagdoll(SpawnRagdollArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBullet(PlaceBulletArgs ev) => ev.IsAllowed = false;
    public void OnPlaceBlood(PlaceBloodArgs ev) => ev.IsAllowed = false;
    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = false;
    public void OnDropAmmo(DropAmmoArgs ev) => ev.IsAllowed = false;
}