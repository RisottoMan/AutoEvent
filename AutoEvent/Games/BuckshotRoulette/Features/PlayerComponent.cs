using System;

namespace AutoEvent.Games.BuckshotRoulette;
public class PlayerComponent
{
    public bool IsKiller { get; set; } = false;
    public bool IsTarget { get; set; } = false;
    public int Kills { get; set; } = 0;
}
