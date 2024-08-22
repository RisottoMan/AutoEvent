using AutoEvent.Interfaces;

namespace AutoEvent.Games.Escape;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Atomic Escape";
    public override string Description { get; set; } = "Escape from the facility as SCP-173 at supersonic speed!";
    public override string CommandName { get; set; } = "escape";
    public string BeforeStart { get; set; } = "{name}\nPrepare to escape from the facility before it explodes!\n<color=red>Time until round starts: {time} seconds</color>";
    public string Cycle { get; set; } = "{name}\n<color=red>{time}</color> seconds until explosion!";
    public string End { get; set; } = "{name}\n<color=red>{players} Players Escaped </color>";
}