namespace AutoEvent.Games.Puzzle;

public static class Preset
{
    public static Config ColorMatch { get; set; } = new Config()
    {
        UseRandomPlatformColors = true,
    };
    public static Config Run { get; set; } = new Config()
    {
        PlatformsOnEachAxis = 10,
        UseRandomPlatformColors = false,
    };
}