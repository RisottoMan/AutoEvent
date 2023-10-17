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
using AutoEvent.API.Components;
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
    public int LayerMask { get; set; } = -1;
}

public class Rock : MonoBehaviour
{
    public void Init(GameObject owner, Footprint thrower, bool friendlyFire = true, float throwDamage = 10, bool explodeOnCollision = false, bool leaveBehindRock = false, bool giveOwnerNewRockOnHit = false, int layerMask = -1)
    {
        
        /*DebugLogger.LogDebug($"OG Layer: {gameObject.layer}. Provided layermask: {layerMask}");
        DebugLogger.LogDebug($"HitMask Layers: {(int)Scp018Projectile.FlybyHitregMask}, {(int)Scp018Projectile.BounceHitregMask}");
        if (layerMask != -1)
            gameObject.layer = layerMask;
        else
            gameObject.layer = Scp018Projectile.FlybyHitregMask;
        DebugLogger.LogDebug($"New Hitmask Layer: {(int)gameObject.layer}");
        */
        
        gameObject.layer = 9;
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
        var collider = gameObject.AddComponent<PlayerCollisionDetectorComponent>();
        collider.PlayerCollision += PlayerCollision;
        if (gameObject.TryGetComponent<Rock>(out _))
            Destroy(this);
    }

    public void PlayerCollision(PlayerCollisionArgs ev)
    {
        try
        {
            var args = _processRockHitArgs(ev.Player.GameObject);
            if (args is null)
            {
                return;
            }
            _giveNewRock(args);
            _destroy();
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"Caught an exception on player collision.", LogLevel.Warn, true);
            DebugLogger.LogDebug($"{e}");
        }
    }

    private RockHitObjectArgs _processRockHitArgs(GameObject gameObject)
    {
        var args = new RockHitObjectArgs(Thrower, gameObject, ThrowDamage, FriendlyFire, ExplodeOnCollision, true, gameObject.layer);
        OnRockHitPlayer(ref args);
        if (!args.IsAllowed)
        {
            return null;
        }
        if (args.Target is not null && args.Target.Team == args.Thrower.Role.GetTeam() && args.FriendlyFire)
        {
            args.IsAllowed = false;
        }
        ProcessDefaultDamage(args);
        if (args.ExplodeOnCollision)
        {
            Vector3 position = gameObject.transform.position;
            ExplosionUtils.ServerExplode(position, Thrower);
        }

        return args;
    }

    private void _giveNewRock(RockHitObjectArgs args)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (LeaveBehindRock )//&& DebugLogger.Debug)
            {
                //DebugLogger.LogDebug("Leave behind rock is a beta feature only enabled in debug mode...");
                // this doesnt work :(
                
                var throwable = Extensions.CreateThrowable(ItemType.SCP018);
                TimeGrenade grenade = (TimeGrenade) Object.Instantiate(throwable.Projectile, gameObject.transform.position, Quaternion.identity);
                grenade.PreviousOwner = new Footprint(args.Thrower.Hub != null ? args.Thrower.Hub : ReferenceHub.HostHub);
                NetworkServer.Spawn(grenade.gameObject);
                if (grenade.TryGetComponent<Scp018Projectile>(out Scp018Projectile ball))
                {
                    // delete the scp018 component.
                    Destroy(ball);
                }
                //DebugLogger.LogDebug($"spawning new Scp018");
                ((Scp018Projectile)grenade).PhysicsModule.DestroyModule();
                
                ((Scp018Projectile)grenade).PhysicsModule = new PickupStandardPhysics(((Scp018Projectile)grenade), PickupStandardPhysics.FreezingMode.FreezeWhenSleeping) { _isFrozen = true, _serverPrevSleeping = true };
                grenade.NetworkInfo = new PickupSyncInfo(ItemType.SCP018, throwable.Weight * 3, throwable.ItemSerial){ Locked = false }; 

            }
            skipSpawn:
            if (GiveOwnerNewRockOnHit && Thrower.Hub is not null && Player.Get(Thrower.Hub) is not null)
            {
                try
                {
                    DebugLogger.LogDebug("Giving player new rock.");

                    Player ply = Player.Get(Thrower.Hub);
                    if (ply.Items.Count < 8)
                    {
                        var item = ply.AddItem(ItemType.SCP018);
                        item.MakeRock(new RockSettings(this.FriendlyFire, this.ThrowDamage, this.ExplodeOnCollision,
                            this.LeaveBehindRock, GiveOwnerNewRockOnHit){ LayerMask = gameObject.layer});
                    }
                    else
                    {
                        DebugLogger.LogDebug("Cannot give rock to player. Full Inventory.");
                    }
                }
                catch (Exception e)
                {
                    DebugLogger.LogDebug($"Could not add a rock to inventory. Reason: \n{e}");
                    // full inv
                }
            }
    }

    private void _destroy()
    {
        Destroy(Owner.gameObject);
        Destroy(this);
        Destroy(gameObject);
    }
    public void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject == Owner)
            {
                return; 
            }

            var args = _processRockHitArgs(collision.contacts[0].otherCollider.gameObject);
            if (args is null)
            {
                return;
            }
            _giveNewRock(args);
            _destroy();
        }
        catch (Exception e)
        {
            DebugLogger.LogDebug($"{nameof(OnCollisionEnter)} error: {e}", LogLevel.Error);
        }
    }

    
    public void ProcessDefaultDamage(RockHitObjectArgs ev)
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
                window.Damage(ev.Damage, new CustomReasonDamageHandler("Rock"), ev.TargetObject.transform.position);
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
            ev.Target.Damage(ev.Damage, "Smashed by a rock .");
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
    public void OnRockHitPlayer(ref RockHitObjectArgs ev) => RockHitPlayerEvent?.Invoke(ev);
    public event Action<RockHitObjectArgs> RockHitPlayerEvent;


}

public class RockHitObjectArgs
{
    public RockHitObjectArgs(Footprint footprint, GameObject collision, float damage, bool friendlyFire,
        bool explodeOnCollision, bool isAllowed = true, int layerMask = -1)
    {
        LayerMask = layerMask;
        IsAllowed = isAllowed;
        Damage = damage;
        FriendlyFire = friendlyFire;
        ExplodeOnCollision = explodeOnCollision;
        Thrower = footprint;
        TargetObject = collision;
        //var refHub = collision.GetComponentInParent<ReferenceHub>();
        Target = Player.Get(collision);
        
        if (Target is not null)
        {
            DebugLogger.LogDebug("Target player for rock found.");
            return;
        }
        
        DebugLogger.LogDebug("Could not get target player for rock.");
    }

    public int LayerMask { get; private set; }
    public Footprint Thrower { get; private set; }
    
    
    [CanBeNull] 
    public Player Target { get; private set; }
    
    public GameObject TargetObject { get; private set; }
    public bool IsAllowed { get; set; }
    public float Damage { get; set; }
    public bool FriendlyFire { get; set; }
    public bool ExplodeOnCollision { get; set; }
}

