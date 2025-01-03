using AutoEvent.Interfaces;

namespace AutoEvent.Games.Airstrike;
public class Translation : EventTranslation
{
    public string Cycle { get; set; } = "<color=yellow>Avoid the airstrikes!</color>\n<color=green>{time} seconds have elapsed</color>\n<color=red>{count} players left</color>";
    public string MorePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow><color=red>{count}</color> players survived.</color>\n<color=#ffc0cb>{time}</color>";
    public string OnePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow>Winner - <color=red>{winner}</color></color>\n<color=#ffc0cb>{time}</color>";
    public string AllDie { get; set; } = "<color=red>Death Party</color>\n<color=yellow>No one survived.(((</color>\n<color=#ffc0cb>{time}</color>";
}