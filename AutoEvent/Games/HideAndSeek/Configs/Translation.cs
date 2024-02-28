using AutoEvent.Interfaces;

namespace AutoEvent.Games.HideAndSeek;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Tag";
    public override string Description { get; set; } = "Tag other players with jailbirds.";
    public override string CommandName { get; set; } = "tag";
    public string Broadcast { get; set; } = "RUN\nTaggers are being selected. Hide now!\nTime remaining: {time}";
    public string Cycle { get; set; } = "Tag another player\n<color=yellow><b><i>{time}</i></b> seconds left</color>";
    public string Hurt { get; set; } = "You didn't tag someone else.";
    public string OnePlayer { get; set; } = "The player won {winner}\n<color=yellow>Event time <color=red>{time}</color></color>";
    public string AllDie { get; set; } = "No one survived.\nEnd of the game\n<color=yellow>Event time <color=red>{time}</color></color>";
}