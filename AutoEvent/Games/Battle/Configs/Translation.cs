using AutoEvent.Interfaces;

namespace AutoEvent.Games.Battle;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Battle";
    public override string Description { get; set; } = "A MTF VS CI battle in an enclosed arena";
    public override string CommandName { get; set; } = "battle";
    public string TimeLeft { get; set; } = "<size=100><color=red>Battle Starting in {time} </color></size>";
    public string CiWin { get; set; } = "<color=#299438>Winner: Chaos Insurgency </color>\n<color=red>Duration: {time} </color>";
    public string MtfWin { get; set; } = "<color=#14AAF5>Winner: Foundation forces</color>\n<color=red>Duration: {time} </color>";
    public string Counter { get; set; } = "<color=#14AAF5> MTF: {FoundationForces} </color> vs <color=#299438> CI: {ChaosForces} </color> \n Elapsed time: {time}";
}