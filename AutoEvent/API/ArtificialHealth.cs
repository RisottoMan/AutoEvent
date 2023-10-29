// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ArtificialHealth.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/23/2023 11:15 PM
//    Created Date:     09/23/2023 11:15 PM
// -----------------------------------------

using System;
using System.ComponentModel;
using PlayerStatsSystem;
using PluginAPI.Core;

namespace AutoEvent.API;

public class ArtificialHealth
{
    public ArtificialHealth(){}

    public ArtificialHealth(float initialHealth, float maxHealth = 100f, float regenerationAmount = 1f, float absorptionPercent = 100, bool permanent = true, float duration = 10f, bool clearOtherInstances = true)
    { 
        InitialAmount = initialHealth;
        MaxAmount = maxHealth;
        RegenerationAmount = regenerationAmount;
        AbsorptionPercent = absorptionPercent;
        Permanent = permanent;
        Duration = duration;
    }

    public void ApplyToPlayer(Player ply)
    {
        if (MaxAmount <= 0)
        {
            return;
        }
        if(ClearOtherInstances)
            ply.GetStatModule<AhpStat>()._activeProcesses.Clear();
        ply.GetStatModule<AhpStat>().ServerAddProcess(
            amount: InitialAmount, 
            limit: MaxAmount, 
            decay: -1*RegenerationAmount, 
            efficacy: AbsorptionPercent / 100f,
            sustain: Duration,
            persistant: Permanent);
    }
    [Description("How much AHP the player will get at first.")]
    public float InitialAmount { get; set; } = 0f;

    [Description("The max amount the player will be able to get.")]
    public float MaxAmount { get; set; } = 100f;

    [Description("The amount of artificial hp the player will get each cycle. 1 is slow, 5 is fast, and 10+ is really fast")]
    public float RegenerationAmount { get; set; } = 1f;

    [Description("The percent of damage that the artificial health will take before effecting the player. (70% = when the player takes 10 damage, 7 of it will be absorbed by ahp, 3 will hurt them directly.)")]
    public float AbsorptionPercent { get; set; } = 100f;
    
    [Description("If set to false, the player will lose this after they take enough damage, or it wears out.")]
    public bool Permanent { get; set; } = true;

    [Description("How long to wait before regenerating.")]
    public float Duration { get; set; } = 30f;

    [Description("Should other instances of AHP, be removed from the player.")]
    public bool ClearOtherInstances { get; set; } = true;
}