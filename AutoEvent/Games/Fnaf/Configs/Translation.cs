using AutoEvent.Interfaces;

namespace AutoEvent.Games.Fnaf;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Five Nights at Freddy's";
    public override string Description { get; set; } = "Survive one night with animatronics";
    public override string CommandName { get; set; } = "fnaf";
}