// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ActiveFeatures.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/24/2023 10:20 PM
//    Created Date:     10/24/2023 10:20 PM
// -----------------------------------------

using System;

namespace AutoEvent;

[Flags]
public enum ActiveFeatures
{
    None = 0,
    Minigames20 = 1,
    Lobby = 2,
    Vote = 4,
    Powerups = 8,
    SchematicApi = 16,
    BuildInfo = 32,
    MinigameSpleef = 64,
    InventoryMenuApi,
    MinigamesGhostBusters,
    All = ~0,
}