// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         FinishWayConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/18/2023 4:08 PM
//    Created Date:     09/18/2023 4:08 PM
// -----------------------------------------

using System.ComponentModel;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Infection;

public class FinishWayConfig : EventConfig
{
    [Description("How long the event should last in seconds. [Default: 60]")]
    public int EventDurationInSeconds = 60;
}