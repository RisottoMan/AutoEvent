using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class LineTranslate : ITranslation 
#else
    public class LineTranslate 
#endif
    {
        public string LineCommandName { get; set; } = "line";
        public string LineName { get; set; } = "DeathLine";
        public string LineDescription { get; set; } = "Avoid the spinning platform to survive.";
        public string LineCycle { get; set; } = "<color=#FF4242>{name}</color>\n<color=#14AAF5>Time remaining: {time}\n<color=yellow>Players Remaining: {count}</color>";
        public string LineMorePlayers { get; set; } = "<color=#FF4242>{name}</color>\n<color=yellow>{count} players survived</color>\n<color=red>Congratulate!</color>";
        public string LineWinner { get; set; } = "<color=#FF4242>{name}</color>\n<color=yellow>Winner: {winner}</color>\n<color=red>Congratulate!</color>";
        public string LineAllDied { get; set; } = "<color=red>All players died</color>";
    }
}