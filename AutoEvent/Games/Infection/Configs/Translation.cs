using AutoEvent.Interfaces;

namespace AutoEvent.Games.Infection;
public class Translation : EventTranslation
{
    public string Start { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=#ABF000>There are <color=red>{time}</color> seconds left before the game starts.</color>";
    public string Cycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>Humans left: <color=green>{count}</color></color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
    public string ExtraTime { get; set; } = "Extra time: {extratime}\n<color=yellow>The <b><i>Last</i></b> person left!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
    public string Win { get; set; } = "<color=red>Zombie Win!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
    public string Lose { get; set; } = "<color=yellow><color=#D71868><b><i>Humans</i></b></color> Win!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
}