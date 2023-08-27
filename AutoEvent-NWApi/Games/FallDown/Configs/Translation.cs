namespace AutoEvent.Games.Infection
{
    public class FallTranslate
    {
        public string FallName { get; set; } = "FallDown";
        public string FallDescription { get; set; } = "All platforms are destroyed. It is necessary to survive";
        public string FallBroadcast { get; set; } = "%name%\n%time%\n<color=yellow>Remaining: </color>%count%<color=yellow> players</color>";
        public string FallWinner { get; set; } = "<color=red>Winner:</color> %winner%";
        public string FallDied { get; set; } = "<color=red>All players died</color>";
    }
}