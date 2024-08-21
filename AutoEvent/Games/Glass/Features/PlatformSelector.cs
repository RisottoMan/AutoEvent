using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.Glass.Features;
public class PlatformSelector
{
    public int PlatformCount { get; set; }
    internal string Seed { get; set; }
    internal List<PlatformData> PlatformData { get; set; }
    public int MinimumSideOffset { get; set; } = 0;
    public int MaximumSideOffset { get; set; } = 0;
    public int LeftSidedPlatforms { get; set; } = 0;
    public int RightSidedPlatforms { get; set; } = 0;

    public PlatformSelector(int platformCount, string salt, int minimumSideOffset, int maximumSideOffset)
    {
        PlatformCount = platformCount;
        PlatformData = new List<PlatformData>();
        MinimumSideOffset = minimumSideOffset;
        MaximumSideOffset = maximumSideOffset;
        Seed = (System.DateTime.Now.Ticks + salt).GetHashCode().ToString();
        _selectPlatformSideCount();
        _createPlatforms();
        _logOutput();
    }
    
    private void _selectPlatformSideCount()
    {
        bool leftSidePriority = Random.Range(0, 2) == 1;
        int percent = Random.Range(MinimumSideOffset, MaximumSideOffset);
        int priority = (int)((float)PlatformCount * ((float)percent / 100f));
        int remainder = PlatformCount - priority;
        LeftSidedPlatforms = leftSidePriority ? priority : remainder;
        RightSidedPlatforms = leftSidePriority ? remainder : priority;
    }

    private void _createPlatforms()
    {
        List<PlatformData> data = new List<PlatformData>();
        
        for (int i = 0; i < LeftSidedPlatforms; i++)
        {
            data.Add(new PlatformData(true, GetIntFromSeededString(Seed, 4, 1 + i)));
        }

        for (int i = 0; i < RightSidedPlatforms; i++)
        {
            data.Add(new PlatformData(false, GetIntFromSeededString(Seed, 4, 1 + i + LeftSidedPlatforms)));
        }

        PlatformData = data.OrderBy(x => x.Placement).ToList();
    }

    private void _logOutput()
    {
        DebugLogger.LogDebug(
            $"Selecting {PlatformCount} Platforms. [{MinimumSideOffset}, {MaximumSideOffset}]   {LeftSidedPlatforms} | {RightSidedPlatforms}",
            LogLevel.Debug, false);
        foreach (var platform in PlatformData.OrderByDescending(x => x.Placement))
        {
            DebugLogger.LogDebug(
                (platform.LeftSideIsDangerous ? "[X] [=]" : "[=] [X]") + $"  Priority: {platform.Placement}",
                LogLevel.Debug, false);
        }
    }
    
    public static int GetIntFromSeededString(string seed, int count, int amount)
    {
        string seedGen = "";
        for (int s = 0; s < count; s++)
        {
            int indexer = (amount * count) + s;
            while (indexer >= seed.Length)
                indexer -= seed.Length - 1;
            seedGen += seed[indexer].ToString();
        }

        return int.Parse(seedGen);
    }
}