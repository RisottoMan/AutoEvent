// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Rock.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/23/2023 3:03 PM
//    Created Date:     09/23/2023 3:03 PM
// -----------------------------------------

using System;
using AutoEvent.Games.Glass.Features;
using Footprinting;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using JetBrains.Annotations;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Items;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace AutoEvent.API;

public class RockSettings
{
    public RockSettings() { }

    public RockSettings(bool friendlyFire = true, float throwDamage = 10f, bool explodeOnCollision = false,
        bool leaveBehindRock = false, bool giveOwnerNewRockOnHit = false)
    {
        FriendlyFire = friendlyFire;
        ThrowDamage = throwDamage;
        ExplodeOnCollision = explodeOnCollision;
        LeaveBehindRock = leaveBehindRock;
        GiveOwnerNewRockOnHit = giveOwnerNewRockOnHit;
    }
    public bool FriendlyFire { get; set; } = true;
    public float ThrowDamage { get; set; } = 10f;
    public bool ExplodeOnCollision { get; set; } = false;
    public bool LeaveBehindRock { get; set; } = false;
    public bool GiveOwnerNewRockOnHit { get; set; } = false;
}

public class Rock : MonoBehaviour
{
    public void Init(GameObject owner, Footprint thrower, bool friendlyFire = true, float throwDamage = 10, bool explodeOnCollision = false, bool leaveBehindRock = false, bool giveOwnerNewRockOnHit = false)
    {
        Owner = owner;
        Thrower = thrower;
        FriendlyFire = friendlyFire;
        ThrowDamage = throwDamage;
        ExplodeOnCollision = explodeOnCollision;
        LeaveBehindRock = leaveBehindRock;
        GiveOwnerNewRockOnHit = giveOwnerNewRockOnHit;
    }

    public void Awake()
    {
        if (gameObject.TryGetComponent<Rock>(out _))
            Destroy(this);
    }


    public void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject == Owner)
            {
                return;
            }

            var args = new RockHitPlayerArgs(Thrower, collision, ThrowDamage, FriendlyFire, ExplodeOnCollision);
            OnRockHitPlayer(ref args);
            if (!args.IsAllowed)
            {
                return;
            }
            if (args.Target is not null && args.Target.Team == args.Thrower.Role.GetTeam() && args.FriendlyFire)
            {
                args.IsAllowed = false;
            }
            ProcessDefaultDamage(args);
            if (args.ExplodeOnCollision)
            {
                Vector3 position = collision.transform.position;
                ExplosionUtils.ServerExplode(collision.GetContact(0).point, Thrower);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (LeaveBehindRock && false)
            {
                // this doesnt work :(
                var throwable = Extensions.CreateThrowable(ItemType.SCP018);
                TimeGrenade grenade = (TimeGrenade) Object.Instantiate(throwable.Projectile, collision.GetContact(0).point, Quaternion.identity);
                grenade.PreviousOwner = new Footprint(args.Thrower.Hub != null ? args.Thrower.Hub : ReferenceHub.HostHub);

                DebugLogger.LogDebug($"spawning new Scp018");
                ((Scp018Projectile)grenade).PhysicsModule.DestroyModule();
                ((Scp018Projectile)grenade).PhysicsModule = new PickupStandardPhysics(((Scp018Projectile)grenade), PickupStandardPhysics.FreezingMode.FreezeWhenSleeping);
                
                NetworkServer.Spawn(grenade.gameObject);
            }
            skipSpawn:
            if (GiveOwnerNewRockOnHit && Thrower.Hub is not null && Player.Get(Thrower.Hub) is not null)
            {
                var item = Player.Get(Thrower.Hub).AddItem(ItemType.SCP018);
                item.MakeRock(new RockSettings(this.FriendlyFire, this.ThrowDamage, this.ExplodeOnCollision, this.LeaveBehindRock, GiveOwnerNewRockOnHit));
            }
            Destroy(Owner.gameObject);
            Destroy(this);
            Destroy(gameObject);
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"{nameof(OnCollisionEnter)} error: {e}", LogLevel.Error);
        }
    }

    
    public void ProcessDefaultDamage(RockHitPlayerArgs ev)
    {
        if (!ev.IsAllowed)
        {
            return;
        }

        if (ev.Target is null)
        {
            if (ev.TargetObject is null)
            {
                return;
            }
            var window = ev.TargetObject.GetComponentInParent<BreakableWindow>();
            var door = ev.TargetObject.GetComponentInParent<BreakableDoor>();
            if (door is null && window is null)
            {
                DebugLogger.LogDebug("Window and door is null.");
                return;
            }

            if (window is not null)
            {
                window.Damage(ev.Damage, new CustomReasonDamageHandler("Rock"), ev.Collision.GetContact(0).point);
            }

            if (door is not null)
            {
                door.ServerDamage(ev.Damage, DoorDamageType.Grenade);
            }
            return;
        }

        if (ev.FriendlyFire && ev.Thrower.Role.GetTeam() == ev.Target.Team)
        {
            return;
        }

        if (ev.Damage > 0)
        {
            ev.Target.Damage(ev.Damage, "A rock has hit you.");
            Player.Get(Thrower.Hub).ReceiveHitMarker(1f);
        }
    }
    public bool ExplodeOnCollision { get; set; }
    public GameObject Owner { get; private set; }
    public Footprint Thrower { get; private set; }
    public bool FriendlyFire { get; set; }
    public float ThrowDamage { get; set; }
    public bool LeaveBehindRock { get; set; }
    public bool GiveOwnerNewRockOnHit { get; set; }
    public void OnRockHitPlayer(ref RockHitPlayerArgs ev) => RockHitPlayerEvent?.Invoke(ev);
    public event Action<RockHitPlayerArgs> RockHitPlayerEvent;


}

public class RockHitPlayerArgs
{
    public RockHitPlayerArgs(Footprint footprint, Collision collision, float damage, bool friendlyFire, bool explodeOnCollision, bool isAllowed = true)
    {
        IsAllowed = isAllowed;
        Collision = collision;
        Damage = damage;
        FriendlyFire = friendlyFire;
        ExplodeOnCollision = explodeOnCollision;
        Thrower = footprint;
        TargetObject = collision.collider.gameObject;
        var refHub = collision.collider.GetComponentInParent<ReferenceHub>();
        if (refHub is not null)
        {
            Target = Player.Get(refHub);
        }
    }

    public Footprint Thrower { get; private set; }
    
    public Collision Collision { get; private set; }
    
    [CanBeNull] 
    public Player Target { get; private set; }
    
    [CanBeNull]
    public GameObject TargetObject { get; private set; }
    public bool IsAllowed { get; set; }
    public float Damage { get; set; }
    public bool FriendlyFire { get; set; }
    public bool ExplodeOnCollision { get; set; }
}

