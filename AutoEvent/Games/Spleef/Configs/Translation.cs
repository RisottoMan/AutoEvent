using AutoEvent.Interfaces;

namespace AutoEvent.Games.Spleef;

public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Spleef";
    public override string Description { get; set; } = "Shoot at the platforms and don't fall into the void";
    public override string CommandName { get; set; } = "spleef";
    public string Start { get; set; } = "<color=red>Starts in: </color>{time}";
    public string Running { get; set; } = "Players Alive: {players}\nTime remaining: {remaining}";
    public string AllDied { get; set; } = "<color=red>All players died</color>\nMini-game ended";
    public string SomeSurvived { get; set; } = "<color=red>Several people survived</color>\nMini-game ended";
    public string Winner { get; set; } = "<color=red>Winner: {winner}</color>\nMini-game ended";
    public string Died { get; set; } = "<color=red>Burned in Lava</color>";
}