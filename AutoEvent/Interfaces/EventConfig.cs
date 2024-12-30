using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API.Enums;
using AutoEvent.API.Season.Enum;

namespace AutoEvent.Interfaces;

public class EventConfig
{
    public EventConfig() { }
    
    [Description("A list of maps that can be used for this event.")]
    public List<MapChance> AvailableMaps { get; set; } = new List<MapChance>();

    [Description("A list of sounds that can be used for this event.")]
    public List<SoundChance> AvailableSounds { get; set; } = new List<SoundChance>();

    [Description("Some plugins may override this out of necessity.")]
    public FriendlyFireSettings EnableFriendlyFireAutoban { get; set; } = FriendlyFireSettings.Default;

    [Description("Some plugins may override this out of necessity.")]
    public FriendlyFireSettings EnableFriendlyFire { get; set; } = FriendlyFireSettings.Default; 
}
public class MapChance
{
    public MapChance() { }

    public MapChance(float chance, MapInfo map, SeasonFlags? flag = SeasonFlags.None)
    {
        Chance = chance;
        Map = map;
        SeasonFlag = flag ?? SeasonFlags.None;
    }
    [Description("The chance of getting this map.")]
    public float Chance { get; set; } = 1f;
    
    [Description("The map and information.")]
    public MapInfo Map { get; set; }

    [Description("Style of this map.")]
    public SeasonFlags SeasonFlag { get; set; } = SeasonFlags.None;
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