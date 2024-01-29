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
using MER.Lite;
using MER.Lite.Objects;
using PluginAPI.Core;
using Powerups.Components;
using UnityEngine;

namespace Powerups;

/// <summary>
/// The abstract implementation used to define powerups.
/// </summary>
public abstract class Powerup
{
    /// <summary>
    /// Initializes a new <see cref="Powerup"/>. 
    /// </summary>
    public Powerup()
    {
        ActivePlayers = new Dictionary<Player, float>();
        Schematics = new List<SchematicObject>();
    }

    /// <summary>
    /// The name of the <see cref="Powerup"/>. If another <see cref="Powerup"/> is found by the same name, it will become "name-1"
    /// </summary>
    public abstract string Name { get; protected set; }

    /// <summary>
    /// A description of the powerup.
    /// </summary>
    public abstract string Description { get; protected set; }

    /// <summary>
    /// How long the <see cref="Powerup"/> should last. Set to -1 to disable the duration system (Non-Reversible Powerups).
    /// </summary>
    public virtual float PowerupDuration { get; protected set; } = -1;
    
    /// <summary>
    /// The name of the schematic to spawn for the <see cref="Powerup"/>.
    /// </summary>
    protected virtual string SchematicName { get; set; }
    
    /// <summary>
    /// The scale of the <see cref="Powerup"/>. This makes the <see cref="SchematicObject"/> bigger or smaller when spawned. 
    /// </summary>
    protected virtual Vector3 SchematicScale { get; set; } = Vector3.one;
    
    /// <summary>
    /// The Scale of the <see cref="BoxCollider"/>. This most likely won't scale the same as the <see cref="SchematicScale"/>, so use this to make the collider bigger / smaller.
    /// </summary>
    protected virtual Vector3 ColliderScale { get; set; } = Vector3.one;
    
    /// <summary>
    /// A list of all <see cref="SchematicObject"/> that give this <see cref="Powerup"/>.
    /// </summary>
    protected List<SchematicObject> Schematics { get; set; }
    
    /// <summary>
    /// A list of <see cref="Player">Players</see> that have this effect active. This can be disabled by setting the <see cref="PowerupDuration"/> to a negative amount.
    /// </summary>
    public Dictionary<Player, float> ActivePlayers { get; internal set; }
    
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
        
        if(API.Debug)
            Log.Debug($"Spawning Powerup {this.Name} at ({position.x}, {position.y}, {position.z}), ({SchematicScale.x * scaleModifer}x{SchematicScale.y* scaleModifer}x{SchematicScale.z* scaleModifer})");
        var schematic = ObjectSpawner.SpawnSchematic(this.SchematicName, position, Quaternion.Euler(Vector3.zero), this.SchematicScale* scaleModifer, false);
        if (schematic is null)
        {
            if(API.Debug)
                Log.Debug($"Schematic is null. Cannot spawn pickup.");
            return;
        }

        if (schematic.gameObject is null)
        {
            if(API.Debug)
                Log.Debug($"Schematic Gameobject is null. Cannot spawn pickup.");
            return;
        }

        var spinningItemComponent = schematic.gameObject.AddComponent<SpinningItemComponent>();
        spinningItemComponent.Speed = 100f;
        var collisionComponent = schematic.gameObject.AddComponent<SchematicCollisionComponent>();
        collisionComponent.SchematicCollision += OnCollision;
        collisionComponent.Init(schematic, ColliderScale * colliderScaleModifier);
        
        Schematics.Add(schematic);
        if(API.Debug)
            Log.Debug($"Schematic position ({schematic.Position.z}, {schematic.Position.y}, {schematic.Position.z}).");
    }

    /// <summary>
    /// Use this to apply this <see cref="Powerup"/> to a player.
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> to apply the <see cref="Powerup"/> to.</param>
    public void ApplyPowerup(Player ply)
    {
        if (PowerupDuration < 0)
            goto OnApplyPowerup;
        if (!ActivePlayers.ContainsKey(ply))
        {
            ActivePlayers.Add(ply, this.PowerupDuration);
        }
        else
        {
            ActivePlayers[ply] = this.PowerupDuration;
        }
        OnApplyPowerup:
        OnApplyPowerup(ply);
    }
    
    /// <summary>
    /// Used to apply a <see cref="Powerup"/> to a player.
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

    
    /// <summary>
    /// Used to remove a <see cref="Player"/> from the effects of the <see cref="Powerup"/>. Not all Powerups can be removed (Ammo).
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> to remove the powerup from.</param>
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

    /// <summary>
    /// Checks if a <see cref="Player"/> has this effect active. Some powerups don't track this, and only apply the powerup once. (Ammo)
    /// </summary>
    /// <param name="ply">The <see cref="Player"/> to check.</param>
    /// <returns></returns>
    public virtual bool PlayerHasEffect(Player ply)
    {
        return this.ActivePlayers.ContainsKey(ply);
    }
}