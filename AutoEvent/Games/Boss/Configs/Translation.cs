using AutoEvent.Interfaces;

namespace AutoEvent.Games.Boss;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Boss Battle";
    public override string Description { get; set; } = "Kill the Boss to win.";
    public override string CommandName { get; set; } = "boss";
    public string TimeLeft { get; set; } = "<size=100><color=red>Starts in {time} </color></size>";
    public string BossWin { get; set; } = "<color=red><b>Boss Wins!</b></color>\n<color=yellow><color=#14AAF5>Humans</color> has been killed and the boss wins with </color>\n<b><color=red>{hp}</color> Hp</b> left";
    public string HumansWin { get; set; } = "<color=#14AAF5>Humans Win!</color>\n<color=yellow><color=red>Boss</color> has been killed with </color>\n<color=#14AAF5>{count}</color> humans left";
    public string Counter { get; set; } = "<color=red><b>{hpbar}</b></color>\n<color=#14AAF5>{count}</color> humans left\n<color=green>{time}</color> seconds left";
}