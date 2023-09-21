namespace AutoEvent.Games.Infection
{
    public class FootballTranslate
    {
        public string FootballName { get; set; } = "Soccer"; // Soccer in america - football everywhere else 🦅🦅🦅🇺🇸🇺🇸🇺🇸 <- (USA Flag doesnt render in rider...)
        public string FootballDescription { get; set; } = "Score 3 goals to win";
        public string FootballRedTeam { get; set; } = "<color=red>Your Team: Red Team\n</color>";
        public string FootballBlueTeam { get; set; } = "<color=#14AAF5>You Team: Blue Team\n</color>";
        public string FootballTimeLeft { get; set; } = "<color=#14AAF5>{BluePnt}</color> : <color=red>{RedPnt}</color>\nTime Remaining: {eventTime}";
        public string FootballRedWins { get; set; } = "<color=red>Red Team Wins</color>";
        public string FootballBlueWins { get; set; } = "<color=#14AAF5>Blue Team Wins</color>";
        public string FootballDraw { get; set; } = "Draw\n<color=#14AAF5>{BluePnt}</color> vs <color=red>{RedPnt}</color>";
    }
}