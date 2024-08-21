using AutoEvent.Interfaces;

namespace AutoEvent.Games.Puzzle;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Puzzle";
    public override string Description { get; set; } = "Get up the fastest on the right color";
    public override string CommandName { get; set; } = "puzzle";
    public string Start { get; set; } = "<b>{name}</b>\n<color=yellow>The mini-game will start in <color=red>{time}</color> seconds</color>";
    public string Stage { get; set; } = "<b>{name}</b>\n<color=red>Stage: </color>{stageNum}<color=red> / </color>{stageFinal}\n<color=yellow>Remaining players:</color> {count}";
    public string AllDied { get; set; } = "<b>{name}</b>\n<color=red>All players died</color>\nMini-game ended";
    public string SomeSurvived { get; set; } = "<b>{name}</b>\n<color=green>{count} players survived</color>\nMini-game ended";
    public string Winner { get; set; } = "<b>{name}</b>\n<color=green>Player {winner} won</color>\nMini-game ended";
    public string Died { get; set; } = "<color=red>Burned in Lava</color>";
}