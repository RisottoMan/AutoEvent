using Exiled.API.Interfaces;

namespace AutoEvent.Games.AllDeathmatch
{
#if EXILED
    public class AllTranslation  : ITranslation 
#else
    public class AllTranslation
#endif
    {
        public string AllCommandName { get; set; } = "dm";
        public string AllName { get; set; } = "All Deathmatch";
        public string AllDescription { get; set; } = "Fight against each other in all deathmatch.";
        public string AllCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>";
        public string AllCounterWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=green>CHAOS</color></color>";
        public string AllTerroristWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>";
    }
}