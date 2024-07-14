using AutoEvent.Interfaces;

namespace AutoEvent.Games.Puzzle;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Puzzle";
    public override string Description { get; set; } = "Get on the correct platform before the others sink into lava";
    public override string CommandName { get; set; } = "puzzle";
    public string Start { get; set; } = "<color=red>Starts in: </color>{time}";
    public string Stage { get; set; } = "<color=red>Stage: </color>{stageNum}<color=red> / </color>{stageFinal}\n<color=yellow>Remaining players:</color> {plyCount}";
    public string AllDied { get; set; } = "<color=red>All players died</color>\nMini-game ended";
    public string SomeSurvived { get; set; } = "<color=red>Several people survived</color>\nMini-game ended";
    public string Winner { get; set; } = "<color=red>Winner: {winner}</color>\nMini-game ended";
    public string Died { get; set; } = "<color=red>You fell into lava</color>";
    public string MainMessage { get; set; } = "<color=#F59F00>P</color><color=#F68523>u</color><color=#F76B46>z</color><color=#F85169>z</color><color=#F9378C>l</color><color=#FA1DAF>e</color>";
}