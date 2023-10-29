// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         GhostBusterClass.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 4:12 PM
//    Created Date:     10/28/2023 4:11 PM
// -----------------------------------------

using System.Linq;
using AutoEvent.Games.GhostBusters.Configs;
using PluginAPI.Core;

namespace AutoEvent.Games.GhostBusters.Features;

public class GhostBusterClass
{
    public GhostBusterClass(Player ply, GhostBusterClassType type)
    {
        Player = ply;
        ClassType = type;
        AbilityCooldown = 0;
    }
    public Player Player { get; set; }
    public GhostBusterClassType ClassType { get; set; }
    public float AbilityCooldown { get; set; }
    public Ability Ability => (AutoEvent.ActiveEvent as GhostBusters.Plugin)!.Abilities.First(x => x.ClassType == ClassType);
    public float AbilityUses { get; set; }

    public bool IsAbilityReady(out string reason)
    {
        reason = "Ability is ready";
        if (Player.IsBypassEnabled)
        {
            reason = "<color=cyan>Bypass Mode Enabled";
        }
        if (Ability is null)
        {
            reason = "No class selected";
            return false;
        }
        if (AbilityUses >= Ability.AllowedUses && Ability.AllowedUses != -1)
        {
            reason = $"You have no more uses remaining \n<b>[{AbilityUses} / {Ability.AllowedUses}]</b>";
            return false;
        }

        if (AbilityCooldown == -1)
        {
            reason = "You ability has been disabled";
            return false;
        }

        if (AbilityCooldown > 0)
        {
            reason = $"Your ability is on cooldown \n<b>{AbilityCooldown}</b> seconds remaining";
            return false;
        }
        
        return true;
    }
}
public enum GhostBusterClassType
{
    HunterUnchosen,
    GhostUnchosen,
    HunterTank,
    HunterMelee,
    HunterSniper,
    GhostInvisibility,
    GhostSpeed,
    GhostExplosive,
    GhostFlash,
    GhostBall,
    GhostLockdown,
}