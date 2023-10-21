// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Powerup.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 12:41 AM
//    Created Date:     10/17/2023 12:41 AM
// -----------------------------------------

using System.Collections.Generic;
using AdminToys;
using AutoEvent.API.Components;
using AutoEvent.API.Schematic;
using AutoEvent.API.Schematic.Objects;
using CedMod.Addons.AdminSitSystem.Commands.Jail;
using MEC;
using Mirror;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.Interfaces;

public abstract class Powerup
{
    public Powerup()
    {
        ActivePlayers = new Dictionary<Player, float>();
        Schematics = new List<SchematicObject>();
    }
    public abstract string Name { get; protected set; }
    public abstract string Description { get; protected set; }
    public virtual float PowerupDuration { get; protected set; } = float.MaxValue;
    protected virtual string SchematicName { get; set; }
    protected virtual Vector3 SchematicScale { get; set; } = Vector3.one;
    protected virtual Vector3 ColliderScale { get; set; } = Vector3.one;
    protected List<SchematicObject> Schematics { get; set; }
    
    internal Dictionary<Player, float> ActivePlayers { get; set; }
    
    /// <summary>
    /// Called when a player collides with the object.
    /// </summary>
    /// <param name="ply">The player colliding with the object.</param>
    protected virtual void OnCollision(Player ply, SchematicObject instance)
    {
        instance.Destroy();
        ApplyPowerup(ply);
    }

    private void OnCollision(SchematicCollisionArgs ev) => OnCollision(ev.Player, ev.Schematic);

    /// <summary>
    /// Used when spawning the pickup at a location.
    /// </summary>
    /// <param name="position">The position to spawn the pickup.</param>
    public virtual void SpawnPickup(Vector3 position, float scaleModifer = 1f, float colliderScaleModifier = 1f)
    {
        // Load the schematic of the powerup.
        
        DebugLogger.LogDebug($"Spawning Powerup {this.Name} at ({position.x}, {position.y}, {position.z}), ({SchematicScale.x * scaleModifer}x{SchematicScale.y* scaleModifer}x{SchematicScale.z* scaleModifer})");
        var schematic = ObjectSpawner.SpawnSchematic(this.SchematicName, position, Quaternion.Euler(Vector3.zero), this.SchematicScale* scaleModifer);
        if (schematic is null)
        {
            DebugLogger.LogDebug($"Schematic is null. Cannot spawn pickup.");
            return;
        }

        if (schematic.gameObject is null)
        {
            DebugLogger.LogDebug($"Schematic Gameobject is null. Cannot spawn pickup.");
            return;
        }

        var spinningItemComponent = schematic.gameObject.AddComponent<SpinningItemComponent>();
        spinningItemComponent.Speed = 100f;
        var collisionComponent = schematic.gameObject.AddComponent<SchematicCollisionComponent>();
        collisionComponent.SchematicCollision += OnCollision;
        collisionComponent.Init(schematic, ColliderScale * colliderScaleModifier);
        
        Schematics.Add(schematic);
        DebugLogger.LogDebug($"Schematic position ({schematic.Position.z}, {schematic.Position.y}, {schematic.Position.z}).");
    }

    public void ApplyPowerup(Player ply)
    {
        if (!ActivePlayers.ContainsKey(ply))
        {
            ActivePlayers.Add(ply, this.PowerupDuration);
        }
        else
        {
            ActivePlayers[ply] = this.PowerupDuration;
        }
        OnApplyPowerup(ply);
    }
    
    /// <summary>
    /// Used to apply a Powerup to a player.
    /// </summary>
    /// <param name="ply">The player to apply it to.</param>
    protected virtual void OnApplyPowerup(Player ply)
    {
        
    }

    /// <summary>
    /// Can be used to automatically remove powerups from players. Override this to disable the auto-duration.
    /// </summary>
    /// <param name="ply">The player who's powerup has expired.</param>
    public virtual void ExpirePowerup(Player ply)
    {
        RemovePowerup(ply);
    }

    public void RemovePowerup(Player ply)
    {
        if (ActivePlayers.ContainsKey(ply))
        { 
            ActivePlayers.Remove(ply);
        }
        OnRemovePowerup(ply);
    }
    
    /// <summary>
    /// Used to remove a powerup from a player.
    /// </summary>
    /// <param name="ply">The player to remove the powerup from.</param>
    protected virtual void OnRemovePowerup(Player ply)
    {
        
    }

    public virtual bool PlayerHasEffect(Player ply)
    {
        return this.ActivePlayers.ContainsKey(ply);
    }
}