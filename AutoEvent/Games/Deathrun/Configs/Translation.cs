using AutoEvent.Interfaces;

namespace AutoEvent.Games.Deathrun;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Death Run";
    public override string Description { get; set; } = "Go to the end, avoiding death-activated trap along the way";
    public override string CommandName { get; set; } = "deathrun";
}