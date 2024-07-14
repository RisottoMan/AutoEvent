using AutoEvent.Interfaces;

namespace AutoEvent.Games.FallDown;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "FallDown";
    public override string Description { get; set; } = "Platforms are destroyed at random. Move between platforms to survive.";
    public override string CommandName { get; set; } = "fall";
    public string Broadcast { get; set; } = "{name}\n{time}\n<color=yellow>Remaining: </color>{count}<color=yellow> players</color>";
    public string Winner { get; set; } = "<color=red>Winner:</color> {winner}";
    public string Died { get; set; } = "<color=red>All players died</color>";
}