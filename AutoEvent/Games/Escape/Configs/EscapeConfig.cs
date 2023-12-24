// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EscapeConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 2:41 PM
//    Created Date:     09/18/2023 2:41 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Escape;

public class EscapeConfig : EventConfig
{
    [Description("How long players have to escape. [Default: 70]")]
    public int EscapeDurationInSeconds { get; set; } = 70;

    public EscapeConfig()
    {
        if (AvailableSounds is null)
        {
            AvailableSounds = new List<SoundChance>();
        }

        if (AvailableSounds.Count < 1)
        {
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Escape.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_NightOfFire.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_ComeOnBaby.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_Dejavu.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_GasGasGas.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_SpeedCar.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_Dancing.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_DancingQueen.ogg", 25)));
            AvailableSounds.Add(new SoundChance(10, new SoundInfo("Initial_D_RunninginThe90s.ogg", 25)));
            AvailableSounds.Add(new SoundChance(5, new SoundInfo("SomeTajikMusic.ogg", 25)));
        }
    }
}