using System.Collections.Generic;
using System.ComponentModel;
using AutoEvent.API;
using AutoEvent.API.Season.Enum;
using AutoEvent.Interfaces;
using UnityEngine;

namespace AutoEvent.Games.Puzzle;
public class Config : EventConfig
{
    public Config()
    {
        if (AvailableMaps is null)
        {
            AvailableMaps = new List<MapChance>();
        }

        if (AvailableMaps.Count < 1)
        {
            AvailableMaps.Add(new MapChance(50, new MapInfo("Puzzle", new Vector3(76f, 1026.5f, -43.68f))));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Puzzle_Xmas2024", new Vector3(76f, 1026.5f, -43.68f)), SeasonFlag.Christmas));
            AvailableMaps.Add(new MapChance(50, new MapInfo("Puzzle_Xmas2024", new Vector3(76f, 1026.5f, -43.68f)), SeasonFlag.NewYear));
        }
    }

    [Description("The number of rounds in the match.")]
    public int Rounds { get; set; } = 10;

    [Description("Salt for the RNG.")] 
    public string Salt { get; set; } = "salty hashbrown";

    [Description("Adds more difficulty towards the end of the game.")]
    public bool UseCustomRoundSystem { get; set; } = true;

    [Description("How fast before the fall delay occurs.")]
    public DifficultyItem FallDelay { get; set; } = new DifficultyItem(5f, 1f);

    [Description("How much time before a selection occurs.")]
    public DifficultyItem SelectionTime { get; set; } = new DifficultyItem(5, 1);
    
    [Description("The number of platforms that will not fall.")]
    public DifficultyItem NonFallingPlatforms { get; set; } = new DifficultyItem(3, 1);
    [Description("How many platforms the x and y axis will have. 5 results in 25 platforms.")]
    public byte PlatformsOnEachAxis { get; set; } = 5;

    [Description("Uses random platform colors instead of green and magenta.")]
    public bool UseRandomPlatformColors { get; set; } = false;
    [Description("How far the platforms are spread out.")]
    public DifficultyItem PlatformSpread { get; set; } = new DifficultyItem(5, 30f);

    [Description("Used for color selection. This will be reworked in the future.")]
    public DifficultyItem HueDifficulty { get; set; } = new DifficultyItem(1, 0.6f);
    [Description("Used for color selection. This will be reworked in the future.")]
    public DifficultyItem SaturationDifficulty { get; set; } = new DifficultyItem(1, 0.6f);
    [Description("Used for color selection. This will be reworked in the future.")]
    public DifficultyItem VDifficulty { get; set; } = new DifficultyItem(1, 0.6f);
}