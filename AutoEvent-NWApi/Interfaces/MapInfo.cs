// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         MapInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/06/2023 5:02 PM
//    Created Date:     09/06/2023 5:02 PM
// -----------------------------------------

using AutoEvent.API.Schematic.Objects;
using UnityEngine;

namespace AutoEvent.Interfaces;

public class MapInfo
{
    public string MapName { get; set; }
    public Vector3 Position { get; set; } = new Vector3(6f, 1030f, -43.5f);
    public Quaternion Rotation { get; set; } = Quaternion.Euler(Vector3.zero);
    public Vector3 Scale { get; set; } = Vector3.one;
    public SchematicObject Map { get; set; }
    public bool SpawnAutomatically { get; set; } = false;
}