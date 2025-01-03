using AutoEvent.Interfaces;

namespace AutoEvent.Games.FallDown;
public class Translation : EventTranslation
{
    public string Broadcast { get; set; } = "{name}\n{time}\n<color=yellow>Remaining: </color>{count}<color=yellow> players</color>";
    public string Winner { get; set; } = "<color=red>Winner:</color> {winner}";
    public string Died { get; set; } = "<color=red>All players died</color>";
}