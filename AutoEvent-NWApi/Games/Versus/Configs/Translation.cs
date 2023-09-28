using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class VersusTranslate : ITranslation 
#else
    public class VersusTranslate
#endif
    {
        public string VersusCommandName { get; set; } = "versus";
        public string VersusName { get; set; } = "Fight Club";
        public string VersusDescription { get; set; } = "Duel of players on the 35hp map from cs 1.6";
        public string VersusPlayersNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nGo inside the arena to fight each other!\n<color=red>{remain}</color> seconds left";
        public string VersusClassDNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nThe player left alive <color=yellow>{scientist}</color>\nGo inside in <color=orange>{remain}</color> seconds";
        public string VersusScientistNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nThe player left alive <color=orange>{classd}</color>\nGo inside in <color=yellow>{remain}</color> seconds";
        public string VersusPlayersDuel { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>";
        public string VersusClassDWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>CLASS D</color></color>";
        public string VersusScientistWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>SCIENTISTS</color></color>";
    }
}