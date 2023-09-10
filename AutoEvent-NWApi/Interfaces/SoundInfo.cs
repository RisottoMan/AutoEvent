// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         SoundInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/06/2023 5:04 PM
//    Created Date:     09/06/2023 5:04 PM
// -----------------------------------------

namespace AutoEvent.Interfaces;

public class SoundInfo
{
    public string SoundName { get; set; }
    public byte Volume { get; set; } = 10;
    public bool Loop { get; set; } = true;
    public bool StartAutomatically { get; set; } = true;
}