namespace AutoEvent.Games.Infection
{
    public class FootballTranslate
    {
        public string FootballName { get; set; } = "Football";
        public string FootballDescription { get; set; } = "Score 3 goals to win";
        public string FootballRedTeam { get; set; } = "<color=red>You play as Red Team\n</color>";
        public string FootballBlueTeam { get; set; } = "<color=#14AAF5>You play as Blue Team\n</color>";
        public string FootballTimeLeft { get; set; } = "<color=#14AAF5>{BluePnt}</color> : <color=red>{RedPnt}</color>\nTime left: {eventTime}";
        public string FootballRedWins { get; set; } = "<color=red>Red Team Wins</color>";
        public string FootballBlueWins { get; set; } = "<color=#14AAF5>Blue Team Wins</color>";
        public string FootballDraw { get; set; } = "Draw\n<color=#14AAF5>{BluePnt}</color> vs <color=red>{RedPnt}</color>";
    }
}