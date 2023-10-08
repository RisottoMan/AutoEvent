// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GunEffect.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/22/2023 6:46 PM
//    Created Date:     09/22/2023 6:46 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API.Enums;
using AutoEvent.Events.EventArgs;
using PlayerStatsSystem;

namespace AutoEvent.API;

public class WeaponEffect
{
    public WeaponEffect() { }

    public WeaponEffect(float damage, List<Effect> effects, ItemType weaponType = ItemType.None)
    {
        Damage = damage;
        Effects = effects;
        WeaponType = weaponType;
    }
    
    [Description("The weapon that must be used for this to apply. If set to none, all weapons will have this applied.")]
    public ItemType WeaponType { get; set; } = ItemType.None;
    
    [Description("The damage that guns do.")]
    public float Damage { get; set; } = 3f;

    [Description("The effects that guns give.")]
    public List<Effect> Effects { get; set; } = new List<Effect>();

    public bool IsCustomWeaponEffect(PlayerDamageArgs args)
    {
        switch (args.DamageHandler)
        {
            case FirearmDamageHandler firearm:
                if (WeaponType != ItemType.None && firearm.WeaponType != WeaponType)
                    return false;
                break;
            case ExplosionDamageHandler explosion:
                if (WeaponType != ItemType.None && WeaponType != ItemType.GrenadeHE)
                    return false;
                break;
            case JailbirdDamageHandler jailbird:
                if (WeaponType != ItemType.None && WeaponType != ItemType.Jailbird)
                    return false;
                break;
            case Scp018DamageHandler ball:
                if (WeaponType != ItemType.None && WeaponType != ItemType.SCP018)
                    return false;
                break;
            case MicroHidDamageHandler hid:
                if (WeaponType != ItemType.None && WeaponType != ItemType.MicroHID)
                    return false;
                break;
            case DisruptorDamageHandler particle:
                if (WeaponType != ItemType.None && WeaponType != ItemType.ParticleDisruptor)
                    return false;
                break;
            default:
                return false;
        }
        return true;
    }
    public void ApplyGunEffect(ref PlayerDamageArgs args, bool runIsAllowedCheck = false)
    {
        if (runIsAllowedCheck && !args.IsAllowed)
        {
            return;
        }

        if (!IsCustomWeaponEffect(args))
        {
            return;
        }

        args.Amount = this.Damage;
        foreach (var effect in this.Effects)
        {
            args.Target.GiveEffect(effect);
        }
    }
}