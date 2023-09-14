// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Loadout.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 1:45 PM
//    Created Date:     09/13/2023 1:45 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace AutoEvent.Games.Example;

[Description("A loadout that a user can get during the event")]
public class Loadout
{
    [Description("The items that this class spawns with.")]
    public List<ItemType> Items { get; set; }

    [Description("The health that this class has.")]
    public int Health { get; set; } = 100;

    [Description("The chance of a user getting this class.")]
    public int Chance { get; set; } = 1;

    [Description("The size of this class.")]
    public Vector3 Size { get; set; } = Vector3.one;
}