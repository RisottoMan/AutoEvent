// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         FallDownConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 2:45 PM
//    Created Date:     09/18/2023 2:45 PM
// -----------------------------------------

using System.ComponentModel;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Infection;

public class FallDownConfig : EventConfig
{
    [Description("The delay between the selection of platforms that fall.")]
    public float DelayInSeconds { get; set; } = 1.0f;

    [Description("Should platforms have a color warning for when they are about to fall.")]
    public bool PlatformsHaveColorWarning = false;

}