using AutoEvent.Interfaces;

namespace AutoEvent.Games.Airstrike;

public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Airstrike Party";
    public override string Description { get; set; } = "Survive as aistrikes rain down from above.";
    public override string CommandName { get; set; } = "airstrike";
    public string DeathCycle { get; set; } = "<color=yellow>Dodge the airstrikes!</color>\n<color=green>{time} seconds have elapsed</color>\n<color=red>{count} players left</color>";
    public string DeathMorePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow><color=red>{count}</color> players survived.</color>\n<color=#ffc0cb>{time}</color>";
    public string DeathOnePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow>Winner - <color=red>{winner}</color></color>\n<color=#ffc0cb>{time}</color>";
    public string DeathAllDie { get; set; } = "<color=red>Death Party</color>\n<color=yellow>No one survived.(((</color>\n<color=#ffc0cb>{time}</color>";
}