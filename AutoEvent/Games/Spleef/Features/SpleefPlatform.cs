// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SpleefPlatform.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/17/2023 6:38 PM
//    Created Date:     10/17/2023 6:38 PM
// -----------------------------------------

using UnityEngine;

namespace AutoEvent.Games.Spleef;

public class SpleefPlatform
{
    public SpleefPlatform(float sizeX, float sizeY, float sizeZ, float positionX, float positionY, float positionZ)
    {
        X = sizeX;
        Y = sizeY;
        Z = sizeZ;
        PositionX = positionX;
        PositionY = positionY;
        PositionZ = positionZ;
    }

    public GameObject GameObject { get; set; }
    public ushort PlatformId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
}