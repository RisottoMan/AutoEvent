namespace AutoEvent.Games.Example
{
    public class ExampleTranslate
    {
        public string BattleName { get; set; } = "Battle";
        public string BattleDescription { get; set; } = "MTF fight against CI in an arena!";
        public string BattleTimeLeft { get; set; } = "<size=100><color=red>Battle Starting in {time} </color></size>";
        public string BattleCiWin { get; set; } = "<color=#299438>Winner: Chaos Insurgency </color>\n<color=red>Duration: {time} </color>";
        public string BattleMtfWin { get; set; } = "<color=#14AAF5>Winner: Foundation forces</color>\n<color=red>Duration: {time} </color>";
        public string BattleCounter { get; set; } = "<color=#14AAF5> MTF: {FoundationForces} </color> vs <color=#299438> CI: {ChaosForces} </color> \n Elapsed time: {time}";
    }
}