// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         InfiniteAmmoComponent.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/19/2023 3:08 PM
//    Created Date:     09/19/2023 3:08 PM
// -----------------------------------------

using UnityEngine;

namespace AutoEvent.API;

public class InfiniteAmmoComponent : Component
{
    public InfiniteAmmoComponent() { }
    public bool EndlessClip { get; set; } = false;
}