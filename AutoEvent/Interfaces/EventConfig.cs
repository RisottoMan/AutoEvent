// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         EventConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 3:32 PM
//    Created Date:     09/13/2023 3:32 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API.Enums;
using YamlDotNet.Serialization;

namespace AutoEvent.Interfaces;

public class EventConfig
{
    public EventConfig()
    {
        
    }
    [YamlIgnore]
    internal string PresetName { get; set; }

    [Description("A list of maps that can be used for this event.")]
    public List<MapChance> AvailableMaps { get; set; } = new List<MapChance>();

    [Description("A list of sounds that can be used for this event.")]
    public List<SoundChance> AvailableSounds { get; set; } = new List<SoundChance>();

    [Description("Some plugins may override this out of necessity.")]
    public FriendlyFireSettings EnableFriendlyFireAutoban { get; set; } = FriendlyFireSettings.Default;

    [Description("Some plugins may override this out of necessity.")]
    public FriendlyFireSettings EnableFriendlyFire { get; set; } = FriendlyFireSettings.Default;
    
    [Description("Should this plugin output debug logs.")]
    public bool Debug { get; set; }
}
public class MapChance
{
    public MapChance() { }

    public MapChance(float chance, MapInfo map)
    {
        Chance = chance;
        Map = map;
    }
    [Description("The chance of getting this map.")]
    public float Chance { get; set; } = 1f;
    
    [Description("The map and information.")]
    public MapInfo Map { get; set; }
}

public class SoundChance
{
    public SoundChance() { }

    public SoundChance(float chance, SoundInfo sound)
    {
        Chance = chance;
        Sound = sound;
    }
    [Description("The chance of getting this sound.")]
    public float Chance { get; set; } = 1f;
    
    [Description("The sound and sound information.")]
    public SoundInfo Sound { get; set; }
}