// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SchematicCollisionComponent.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 12:53 AM
//    Created Date:     10/17/2023 12:53 AM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using AutoEvent.API.Schematic;
using AutoEvent.API.Schematic.Objects;
using Mirror;
using PlayerStatsSystem;
using PluginAPI.Core;
using UnityEngine;
using Object = YmlNavigator.Object;

namespace AutoEvent.API.Components;

public class SchematicCollisionComponent : MonoBehaviour
{
    private BoxCollider collider;
    private Vector3 ColliderScale;
    private SchematicObject schematic;
    public void Init(SchematicObject schematicObject, Vector3 colliderScale)
    {
        schematic = schematicObject;
        ColliderScale = colliderScale;
    }
    private void Start()
    {
        collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = ColliderScale;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Player.Get(other.gameObject) is not Player ply)
        {
            return;
        }

        SchematicCollision?.Invoke(new SchematicCollisionArgs(ply, this.gameObject, schematic));
    }
    public event Action<SchematicCollisionArgs> SchematicCollision;
}

public class SchematicCollisionArgs
{
    /// <summary>
    /// Initializes an instance of <see cref="SchematicCollisionArgs"/>
    /// </summary>
    /// <param name="ply">The player colliding with the schematic.</param>
    public SchematicCollisionArgs(Player ply, GameObject gameObject, SchematicObject schematic)
    {
        Player = ply;
        GameObject = gameObject;
        Schematic = schematic;
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
    /// The instance of the Schematic which was collided with.
    /// </summary>
    public SchematicObject Schematic { get; init; }
}