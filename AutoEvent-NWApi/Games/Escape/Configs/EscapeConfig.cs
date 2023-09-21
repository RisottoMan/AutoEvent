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

using System.ComponentModel;
using AutoEvent.Interfaces;

namespace AutoEvent.Games.Infection;

public class EscapeConfig : EventConfig
{
    [Description("How long players have to escape. [Default: 80]")]
    public int EscapeDurationInSeconds { get; set; } = 80;
}