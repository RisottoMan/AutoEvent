namespace AutoEvent.Games.Infection
{
    public class BattleTranslate
    {
        public string BattleName { get; set; } = "Battle";
        public string BattleDescription { get; set; } = "MTF fight against CI";
        public string BattleTimeLeft { get; set; } = "<size=100><color=red>Starts in {time} </color></size>";
        public string BattleCiWin { get; set; } = "<color=#299438>Winner: Chaos Insurgency </color>\n<color=red>Event time: {time} </color>";
        public string BattleMtfWin { get; set; } = "<color=#14AAF5>Winner: Foundation forces</color>\n<color=red>Event time: {time} </color>";
        public string BattleCounter { get; set; } = "<color=#14AAF5> MTF: {FoundationForces} </color> vs <color=#299438> CI: {ChaosForces} </color> \n Elapsed time: {time}";
    }
}