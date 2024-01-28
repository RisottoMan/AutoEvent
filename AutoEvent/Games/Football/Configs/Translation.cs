using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class FootballTranslate : ITranslation
#else
    public class FootballTranslate
#endif
    {
        public string FootballCommandName { get; set; } = "football";
        public string FootballName { get; set; } = "Football";
        public string FootballDescription { get; set; } = "Score 3 goals to win";
        public string FootballRedTeam { get; set; } = "<color=red>Your Team: Red Team\n</color>";
        public string FootballBlueTeam { get; set; } = "<color=#14AAF5>You Team: Blue Team\n</color>";
        public string FootballTimeLeft { get; set; } = "<color=#14AAF5>{BluePnt}</color> : <color=red>{RedPnt}</color>\nTime Remaining: {time}";
        public string FootballRedWins { get; set; } = "<color=red>Red Team Wins</color>";
        public string FootballBlueWins { get; set; } = "<color=#14AAF5>Blue Team Wins</color>";
        public string FootballDraw { get; set; } = "Draw\n<color=#14AAF5>{BluePnt}</color> vs <color=red>{RedPnt}</color>";
    }
}