using AutoEvent.Interfaces;

namespace AutoEvent.Games.Spleef;
public class Translation : EventTranslation
{
    public string Start { get; set; } = "<color=red>Starts in: </color>{time}";
    public string Cycle { get; set; } = "<color=red>{name}</color>\n<color=yellow>Players Alive:</color> {players}\n<color=#42aaff>Time remaining:</color> {remaining}";
    public string AllDied { get; set; } = "<color=red>All players died</color>\nMini-game ended";
    public string SomeSurvived { get; set; } = "<color=red>Several people survived</color>\nMini-game ended";
    public string Winner { get; set; } = "<color=red>Winner: {winner}</color>\nMini-game ended";
    public string Died { get; set; } = "<color=red>You fell into lava</color>";
}