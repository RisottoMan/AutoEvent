using AutoEvent.Interfaces;

namespace AutoEvent.Games.AmongUs;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Buckshot Roulette";
    public override string Description { get; set; } = "One-on-one battle in Russian roulette with shotguns";
    public override string CommandName { get; set; } = "shotgun";
}