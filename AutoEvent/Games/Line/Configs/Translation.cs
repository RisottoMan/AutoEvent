using AutoEvent.Interfaces;

namespace AutoEvent.Games.Line;
public class Translation : EventTranslation
{
    public string Cycle { get; set; } = "<color=#FF4242>{name}</color>\n<color=#14AAF5>Time remaining: {time}\n<color=yellow>Players Remaining: {count}</color>";
    public string MorePlayers { get; set; } = "<color=#FF4242>{name}</color>\n<color=yellow>{count} players survived</color>\n<color=red>Congratulations!</color>";
    public string Winner { get; set; } = "<color=#FF4242>{name}</color>\n<color=yellow>Winner: {winner}</color>\n<color=red>Congratulations!</color>";
    public string AllDied { get; set; } = "<color=red>All players died</color>";
}