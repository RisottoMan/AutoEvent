using AutoEvent.Interfaces;

namespace AutoEvent.Games.Football;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Football";
    public override string Description { get; set; } = "Score 3 goals to win";
    public override string CommandName { get; set; } = "football";
    public string RedTeam { get; set; } = "<color=red>Your Team: Red Team\n</color>";
    public string BlueTeam { get; set; } = "<color=#14AAF5>You Team: Blue Team\n</color>";
    public string TimeLeft { get; set; } = "<color=#14AAF5>{BluePnt}</color> : <color=red>{RedPnt}</color>\nTime Remaining: {time}";
    public string RedWins { get; set; } = "<color=red>Red Team Wins</color>";
    public string BlueWins { get; set; } = "<color=#14AAF5>Blue Team Wins</color>";
    public string Draw { get; set; } = "Draw\n<color=#14AAF5>{BluePnt}</color> vs <color=red>{RedPnt}</color>";
}