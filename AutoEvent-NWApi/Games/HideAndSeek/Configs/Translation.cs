namespace AutoEvent.Games.Infection
{
    public class HideTranslate
    {
        public string HideName { get; set; } = "Tag";
        public string HideDescription { get; set; } = "We need to catch up with all the players on the map.";
        public string HideBroadcast { get; set; } = "RUN\nSelection of new catching up players.\n%time%";
        public string HideCycle { get; set; } = "Tag another player\n<color=yellow><b><i>%time%</i></b> seconds left</color>";
        public string HideHurt { get; set; } = "You didn't have time to tag someone else.";
        public string HideMorePlayer { get; set; } = "There are a lot of players left.\nWaiting for a reboot.\n<color=yellow>Event time <color=red>%time%</color></color>";
        public string HideOnePlayer { get; set; } = "The player won %winner%\n<color=yellow>Event time <color=red>%time%</color></color>";
        public string HideAllDie { get; set; } = "No one survived.\nEnd of the game\n<color=yellow>Event time <color=red>%time%</color></color>";
    }
}