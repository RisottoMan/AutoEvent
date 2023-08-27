namespace AutoEvent.Games.Infection
{
    public class EscapeTranslate
    {
        public string EscapeName { get; set; } = "Atomic Escape";
        public string EscapeDescription { get; set; } = "Escape from the facility behind SCP-173 at supersonic speed!";
        public string EscapeBeforeStart { get; set; } = "{name}\nHave time to escape from the facility before it explodes!\n<color=red>Before the escape: {time} seconds</color>";
        public string EscapeCycle { get; set; } = "{name}\nBefore the explosion: <color=red>{time}</color> seconds";
        public string EscapeEnd { get; set; } = "{name}\n<color=red> SCP Win </color>";
    }
}