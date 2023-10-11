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

            var args = new RockHitPlayerArgs(Thrower, collision, ThrowDamage, FriendlyFire, ExplodeOnCollision, true, gameObject.layer);
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
            if (LeaveBehindRock && DebugLogger.Debug)
            {
                DebugLogger.LogDebug("Leave behind rock is a beta feature only enabled in debug mode...");
                // this doesnt work :(
                
                var throwable = Extensions.CreateThrowable(ItemType.SCP018);
                TimeGrenade grenade = (TimeGrenade) Object.Instantiate(throwable.Projectile, collision.GetContact(0).point, Quaternion.identity);
                grenade.PreviousOwner = new Footprint(args.Thrower.Hub != null ? args.Thrower.Hub : ReferenceHub.HostHub);
                NetworkServer.Spawn(grenade.gameObject);
                if (grenade.TryGetComponent<Scp018Projectile>(out Scp018Projectile ball))
                {
                    // delete the scp018 component.
                    Destroy(ball);
                }
                DebugLogger.LogDebug($"spawning new Scp018");
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
    public void OnRockHitPlayer(ref RockHitPlayerArgs ev) => RockHitPlayerEvent?.Invoke(ev);
    public event Action<RockHitPlayerArgs> RockHitPlayerEvent;


}

public class RockHitPlayerArgs
{
    public RockHitPlayerArgs(Footprint footprint, Collision collision, float damage, bool friendlyFire, bool explodeOnCollision, bool isAllowed = true, int layerMask = -1)
    {
        LayerMask = layerMask;
        IsAllowed = isAllowed;
        Collision = collision;
        Damage = damage;
        FriendlyFire = friendlyFire;
        ExplodeOnCollision = explodeOnCollision;
        Thrower = footprint;
        TargetObject = collision.collider.gameObject;
        if (DebugLogger.Debug)
        {

            var refHub = collision.collider.GetComponentInParent<ReferenceHub>();

            if (refHub is not null)
            {
                Target = Player.Get(refHub);
                DebugLogger.LogDebug("Target player for rock found.");
                return;
            }

            var gameObject = collision.collider.transform.root.gameObject;
            Player ply = Player.Get(gameObject);
            if (ply is not null)
            {
                DebugLogger.LogDebug("Target player for rock found.");
                Target = ply;
                return;
            }

            ReferenceHub hub = Collision.collider.GetComponentInParent<ReferenceHub>();
            int a = Collision.GetContact(0).thisCollider.gameObject.layer;
            int b = Collision.GetContact(0).otherCollider.gameObject.layer;
            DebugLogger.LogDebug($"A: {a}, B: {b}");


            /*foreach (var transform in collider.GetComponentsInParent<ReferenceHub>())
            {
                foreach (var obj in transform.GetComponentsInParent<Component>())
                {
                    DebugLogger.LogDebug($"Component: {obj.GetType().Name}{obj.name}");
                }
            }*/

            /*if (collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                DebugLogger.LogDebug($"Found Destructible");
            }

            if (collider.TryGetComponent<HitboxIdentity>(out HitboxIdentity identity))
            {
                DebugLogger.LogDebug($"Found Hitbox");
            }
            */
            /*DebugLogger.LogDebug("============ Direct Components ============");
                foreach (var x in collider.GetComponents<Component>())
                {
                    DebugLogger.LogDebug($"{x.GetType().Name}");
                }
                DebugLogger.LogDebug("============ Parent Components ============");
                foreach (var x in collider.GetComponentsInParent<Component>())
                {
                    DebugLogger.LogDebug($"{x.GetType().Name}");
                }*/
            /*var refHub = collision.collider.GetComponentInParent<ReferenceHub>();
        if (refHub is not null)
        {
            var player x = Player.Get(refHub);
        }*/
            DebugLogger.LogDebug(
                $"HitboxIdentity: {collision.collider.GetComponentInParent<HitboxIdentity>() is not null}");
            DebugLogger.LogDebug(
                $"IDestructible: {collision.collider.GetComponentInParent<IDestructible>() is not null}");
            DebugLogger.LogDebug($"RefHub: {collision.collider.GetComponentInParent<ReferenceHub>() is not null}");
            DebugLogger.LogDebug(
                $"HitboxIdentity: {collision.contacts[0].otherCollider.GetComponentInParent<HitboxIdentity>() is not null}");
            DebugLogger.LogDebug(
                $"IDestructible: {collision.contacts[0].otherCollider.GetComponentInParent<IDestructible>() is not null}");
            DebugLogger.LogDebug(
                $"RefHub: {collision.contacts[0].otherCollider.GetComponentInParent<ReferenceHub>() is not null}");


            int i = 0;
            // foreach (var collider in collision.contacts)
            foreach (var collider in Physics.OverlapSphere(collision.GetContact(0).point, 1f,
                         layerMask == -1 ? 8 : layerMask))
            {
                //DebugLogger.LogDebug($"User: {collider.GetComponentInParent<ReferenceHub>().nicknameSync.DisplayName}");
                // DebugLogger.LogDebug($"User: {collider.GetComponentInParent<ReferenceHub>().nicknameSync.DisplayName}");



                DebugLogger.LogDebug($"RefHub: {collider.TryGetComponent<ReferenceHub>(out _)}");
                DebugLogger.LogDebug($"SafeTeleportPosition: {collider.TryGetComponent<SafeTeleportPosition>(out _)}");
                DebugLogger.LogDebug($"Transform: {collider.TryGetComponent<Transform>(out _)}");
                DebugLogger.LogDebug($"IDestructible: {collider.TryGetComponent<IDestructible>(out _)}");
                DebugLogger.LogDebug($"HitboxIdentity: {collider.TryGetComponent<HitboxIdentity>(out _)}");
                DebugLogger.LogDebug($"HitboxIdentity: {collider.GetComponentInParent<HitboxIdentity>() is not null}");
                DebugLogger.LogDebug($"IDestructible: {collider.GetComponentInParent<IDestructible>() is not null}");
                DebugLogger.LogDebug($"RefHub: {collider.GetComponentInParent<ReferenceHub>() is not null}");
                DebugLogger.LogDebug(
                    $"SafeTeleportPosition: {collider.GetComponentInParent<SafeTeleportPosition>() is not null}");
                DebugLogger.LogDebug($"Transform: {collider.GetComponentInParent<Transform>() is not null}");
                foreach (var transform in collider.GetComponents<Transform>())
                {
                    DebugLogger.LogDebug($"Transform: {transform.transform.position}");
                }

                foreach (var transform in collider.GetComponentsInParent<Transform>())
                {
                    DebugLogger.LogDebug($"Parent Transform: {transform.transform.position}");
                }

                foreach (var destructible in collider.GetComponents<IDestructible>())
                {
                    DebugLogger.LogDebug($"IDestructible: {destructible.NetworkId}");
                }

                foreach (var destructible in collider.GetComponentsInParent<IDestructible>())
                {
                    DebugLogger.LogDebug($"Parent IDestructible: {destructible.NetworkId}");
                }

                foreach (var hitbox in collider.GetComponents<HitboxIdentity>())
                {
                    DebugLogger.LogDebug($"Hitbox: {hitbox.NetworkId}");
                }

                foreach (var hitbox in collider.GetComponentsInParent<HitboxIdentity>())
                {
                    DebugLogger.LogDebug($"Parent Hitbox: {hitbox.NetworkId}");
                }

                foreach (var tpPos in collider.GetComponents<SafeTeleportPosition>())
                {
                    DebugLogger.LogDebug($"TP Pos: {tpPos.SafePositions.Length}");
                }

                foreach (var tpPos in collider.GetComponentsInParent<SafeTeleportPosition>())
                {
                    DebugLogger.LogDebug($"Parent Tp Pos: {tpPos.SafePositions.Length}");
                }


                DebugLogger.LogDebug($"============ Direct Components (collider {i}) ============");
                foreach (var x in collider.GetComponents<Component>())
                {
                    DebugLogger.LogDebug($"{x.GetType().Name}");
                }

                DebugLogger.LogDebug($"============ Parent Components (collider {i}) ============");
                foreach (var x in collider.GetComponentsInParent<Component>())
                {
                    DebugLogger.LogDebug($"{x.GetType().Name}");
                }

                i++;
            }


#if EXILED
            //var exPly3 = Exiled.API.Features.Player.Get(collision.gameObject.transform);
            var exPly1 = Exiled.API.Features.Player.Get(collision.gameObject);
            var exPly2 = Exiled.API.Features.Player.Get(collision.collider);
            DebugLogger.LogDebug($"Player: {exPly1 is not null}, {exPly2 is not null} ");
#endif
            DebugLogger.LogDebug("Could not get target player for rock.");
        }
    }

    public int LayerMask { get; private set; }
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

