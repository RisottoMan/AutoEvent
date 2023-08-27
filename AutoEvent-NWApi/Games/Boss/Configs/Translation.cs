namespace AutoEvent.Games.Infection
{
    public class BossTranslate
    {
        public string BossName { get; set; } = "Boss Battle";
        public string BossDescription { get; set; } = "You need kill the Boss.";
        public string BossTimeLeft { get; set; } = "<size=100><color=red>Starts in {time} </color></size>";
        public string BossWin { get; set; } = "<color=red><b>Boss WIN</b></color>\n<color=yellow><color=#14AAF5>Humans</color> has been destroyed</color>\n<b><color=red>%hp%</color> Hp</b> left";
        public string BossHumansWin { get; set; } = "<color=#14AAF5>Humans WIN</color>\n<color=yellow><color=red>Boss</color> has been destroyed</color>\n<color=#14AAF5>%count%</color> players left";
        public string BossCounter { get; set; } = "<color=red><b>%hp% HP</b></color>\n<color=#14AAF5>%count%</color> players left\n<color=green>%time%</color> seconds left";
    }
}