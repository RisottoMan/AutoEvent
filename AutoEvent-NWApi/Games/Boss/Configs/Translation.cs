using Exiled.API.Interfaces;

namespace AutoEvent.Games.Boss
{
#if EXILED
    public class BossTranslate  : ITranslation 
#else
    public class BossTranslate 
#endif
    {
        public string BossCommandName { get; set; } = "boss";
        public string BossName { get; set; } = "Boss Battle";
        public string BossDescription { get; set; } = "Kill the Boss to win.";
        public string BossTimeLeft { get; set; } = "<size=100><color=red>Starts in {time} </color></size>";
        public string BossWin { get; set; } = "<color=red><b>Boss Wins!</b></color>\n<color=yellow><color=#14AAF5>Humans</color> has been killed and the boss wins with </color>\n<b><color=red>{hp}</color> Hp</b> left";
        public string BossHumansWin { get; set; } = "<color=#14AAF5>Humans Win!</color>\n<color=yellow><color=red>Boss</color> has been killed with </color>\n<color=#14AAF5>{count}</color> humans left";
        public string BossCounter { get; set; } = "<color=red><b>Boss HP: {hp} HP</b></color>\n<color=#14AAF5>{count}</color> humans left\n<color=green>{time}</color> seconds left";
    }
}