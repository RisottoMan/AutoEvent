// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DeathPartyConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/17/2023 10:50 PM
//    Created Date:     09/17/2023 10:50 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.Interfaces;
using MEC;
using YamlDotNet.Serialization;

namespace AutoEvent.Games.Infection;

[Description("Be aware that this plugin can cause lag if not carefully balanaced.")]
public class DeathPartyConfig : EventConfig
{
    [Description("How many grenades will spawn in the round from 1 - 300. [Default: 20 - 110]")]
    public DifficultyItem DifficultyCount { get; set; } = new DifficultyItem(20, 110);
    
    [Description("How fast grenades will appear from 0 - 5. [Default: 1 - 0.25]")]
    public DifficultyItem DifficultySpeed { get; set; } = new DifficultyItem(1f, 0.1f);

    [Description("How high the grenades should spawn from 0 - 30. [Default: 20 - 5]")]
    public DifficultyItem DifficultyHeight { get; set; } = new DifficultyItem(20, 5);

    [Description("How long the grenade fuse is, from when grenades spawn. from 2 - 10. [Default: 10 - 4]")] 
    public DifficultyItem DifficultyFuseTime { get; set; } = new DifficultyItem(10, 4);
    
    [Description("How large should grenades become from 0.1 to 75. [Default: 4 - 1]")]
    public DifficultyItem DifficultyScale { get; set; } = new DifficultyItem(4, 1);
    
    [Description("How far from center should grenades spawn from 1 to 30. [Default: 4 - 25]")]
    public DifficultyItem DifficultySpawnRadius { get; set; } = new DifficultyItem(4, 25);
    
    [Description("Should grenades spawn on top of randomly chosen players. This will not apply on the last round.")]
    public bool TargetPlayers { get; set; } = false;

    [Description("The amount of rounds that this gamemode lasts. The last round is always a super big grenade.")] 
    public int Rounds { get; set; } = 5;

}

public enum Filter
{
    Linear,
    //Quadratic,
    //Exponential,
}