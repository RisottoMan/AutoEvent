using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class EscapeTranslate : ITranslation 
#else
    public class EscapeTranslate
#endif
    {
        public string EscapeCommandName { get; set; } = "escape";
        public string EscapeName { get; set; } = "Atomic Escape";
        public string EscapeDescription { get; set; } = "Escape from the facility behind SCP-173 at supersonic speed!";
        public string EscapeBeforeStart { get; set; } = "{name}\nPrepare to escape from the facility before it explodes!\n<color=red>Time until round starts: {time} seconds</color>";
        public string EscapeCycle { get; set; } = "{name}\n<color=red>{time}</color> seconds until explosion!";
        public string EscapeEnd { get; set; } = "{name}\n<color=red>{players} Players Escaped </color>";
    }
}