namespace AutoEvent.Games.Infection
{
    public class LineTranslate
    {
        public string LineName { get; set; } = "DeathLine";
        public string LineDescription { get; set; } = "Avoid the spinning platform to survive.";
        public string LineCycle { get; set; } = "<color=#FF4242>%name%</color>\n<color=#14AAF5>Time to end: %min%</color><color=#4a4a4a>:</color><color=#14AAF5>%sec%</color>\n<color=yellow>Players: %count%</color>";
        public string LineMorePlayers { get; set; } = "<color=#FF4242>%name%</color>\n<color=yellow>%count% players survived</color>\n<color=red>Congratulate!</color>";
        public string LineWinner { get; set; } = "<color=#FF4242>%name%</color>\n<color=yellow>Winner: %winner%</color>\n<color=red>Congratulate!</color>";
        public string LineAllDied { get; set; } = "<color=red>All players died</color>";
    }
}