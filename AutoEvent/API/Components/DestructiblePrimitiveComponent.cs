// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DestructiblePrimitiveComponent.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 6:46 PM
//    Created Date:     10/17/2023 6:46 PM
// -----------------------------------------

using System;
using AdminToys;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace AutoEvent.API.Components;

public class DestructiblePrimitiveComponent : UnityEngine.MonoBehaviour, IDestructible
{
    public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
    {
        var ev = new DamagingPrimitiveArgs(damage, handler, exactHitPos);
        DamagingPrimitive?.Invoke(ev);
        if (!ev.IsAllowed)
        {
            return false;
        }

        Health -= ev.Damage;
        if (Health <= 0)
        {
            
            var prim = gameObject.GetComponent<PrimitiveObjectToy>();
            NetworkServer.UnSpawn(base.gameObject);
            Destroy(gameObject);
        }

        return true;
    }

    public uint NetworkId { get; }
    public Vector3 CenterOfMass { get; }
    public float Health { get; private set; }
    public event Action<DamagingPrimitiveArgs> DamagingPrimitive;

}

public class DamagingPrimitiveArgs
{
    public DamagingPrimitiveArgs(float damage, DamageHandlerBase handler, Vector3 exactHitPos, bool isAllowed = true)
    {
        Damage = damage;
        Handler = handler;
        ExactHitPosition = exactHitPos;
        IsAllowed = isAllowed;
    }
    public float Damage { get; set; }
    public DamageHandlerBase Handler { get; init; }
    public Vector3 ExactHitPosition { get; set; }
    public bool IsAllowed { get; set; }
    
}