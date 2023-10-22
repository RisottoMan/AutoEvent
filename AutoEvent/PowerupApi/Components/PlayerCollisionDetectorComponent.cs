// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         PlayerCollisionDetectorComponent.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 5:36 PM
//    Created Date:     10/17/2023 5:36 PM
// -----------------------------------------

using System;
using PluginAPI.Core;
using UnityEngine;

namespace Powerups.Components;

public class PlayerCollisionDetectorComponent : BaseCollisionDetectorComponent
{
    public override float RaycastDistance { get; set; } = 1f;
    public override float Delay { get; set; } = 2;
    public Player? Player { get; private set; }

    public void Init(Player? ply = null)
    {
        Player = ply;
    }
    public override void OnCollisionWithSomething(GameObject gameObject)
    {
        if (!IsPlayer(gameObject, out var player))
        {
            return;
        }
        
        PlayerCollision.Invoke(new PlayerCollisionArgs(player, gameObject));
    }
    public event Action<PlayerCollisionArgs> PlayerCollision;

    public void Disable()
    {
        Destroy(this);
    } 
}
public class PlayerCollisionArgs
{
    /// <summary>
    /// Initializes an instance of <see cref="SchematicCollisionArgs"/>
    /// </summary>
    /// <param name="ply">The player colliding with the schematic.</param>
    public PlayerCollisionArgs(Player ply, GameObject gameObject)
    {
        Player = ply;
        GameObject = gameObject;
    }
    
    /// <summary>
    /// The player that collided with the component.
    /// </summary>
    public Player Player { get; init; }
    
    /// <summary>
    /// The instance of the gameobject which was collided with.
    /// </summary>
    public GameObject GameObject { get; init; }
    
    /// <summary>
    /// If the original object was a player, it will be present here. Otherwise it will be null.
    /// </summary>
    public Player? OriginalPlayer { get; init; }
}