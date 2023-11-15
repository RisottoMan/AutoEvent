using Exiled.API.Interfaces;

namespace AutoEvent.Games.Trouble
{
#if EXILED
    public class TroubleTranslation : ITranslation 
#else
    public class TroubleTranslation 
#endif
    {
        public string TroubleCommandName { get; set; } = "trouble";
        public string TroubleName { get; set; } = "Trouble in Terrorist Town";
        public string TroubleDescription { get; set; } = "An impostor appeared in terrorist town.";
        public string TroubleBeforeStart { get; set; } = "<color=red>The trailer will appear in <color=yellow>{time}</color> seconds.</color>";
        public string TroubleCycle { get; set; } = "<color=red>{name}\n{scp} traitors | <color=#00FFFF>{human} guys</color></color>";
        public string TroubleEveryoneDied { get; set; } = "<color=red>Draw!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
        public string TroubleTraitorWin { get; set; } = "<color=red>Traitors Win!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
        public string TroubleHumanWin { get; set; } = "<color=yellow><color=#D71868><b><i>Humans</i></b></color> Win!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
    }
}