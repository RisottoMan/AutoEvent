namespace AutoEvent.Games.Infection
{
    public class KnivesTranslate
    {
        public string KnivesName { get; set; } = "Knives of Death";
        public string KnivesDescription { get; set; } = "Knife players against each other on a 35hp map from cs 1.6";
        public string KnivesCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow><color=blue>{mtfcount} MTF</color> <color=red>VS</color> <color=green>{chaoscount} CHAOS</color></color>";
        public string KnivesChaosWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=green>CHAOS</color></color>";
        public string KnivesMtfWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>";
    }
}