// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         IEventMap.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/06/2023 4:10 PM
//    Created Date:     09/06/2023 4:10 PM
// -----------------------------------------

using AutoEvent.API.Schematic.Objects;
using UnityEngine;

namespace AutoEvent.Interfaces;

public interface IEventMap
{
    MapInfo MapInfo { get; set; }
}