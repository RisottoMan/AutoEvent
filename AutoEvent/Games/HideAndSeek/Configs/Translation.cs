using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class HideTranslate : ITranslation 
#else
    public class HideTranslate
#endif

    {
        public string HideCommandName { get; set; } = "tag";
        public string HideName { get; set; } = "Tag";
        public string HideDescription { get; set; } = "We need to catch up with all the players on the map.";
        public string HideBroadcast { get; set; } = "RUN\nTaggers are being selected. Hide now!\nTime remaining: {time}";
        public string HideCycle { get; set; } = "Tag another player\n<color=yellow><b><i>{time}</i></b> seconds left</color>";
        public string HideHurt { get; set; } = "You didn't have time to tag someone else.";
        public string HideOnePlayer { get; set; } = "The player won {winner}\n<color=yellow>Event time <color=red>{time}</color></color>";
        public string HideAllDie { get; set; } = "No one survived.\nEnd of the game\n<color=yellow>Event time <color=red>{time}</color></color>";
    }
}