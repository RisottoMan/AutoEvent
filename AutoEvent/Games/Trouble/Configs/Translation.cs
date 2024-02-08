using AutoEvent.Interfaces;

namespace AutoEvent.Games.Trouble;

public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Trouble in Terrorist Town";
    public override string Description { get; set; } = "An impostor appeared in terrorist town";
    public override string CommandName { get; set; } = "trouble";
    public string TroubleBeforeStart { get; set; } = "<color=red>The trailer will appear in <color=yellow>{time}</color> seconds.</color>";
    public string TroubleCycle { get; set; } = "<color=red>{name}\n{scp} traitors | <color=#00FFFF>{human} guys</color></color>";
    public string TroubleEveryoneDied { get; set; } = "<color=red>Draw!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
    public string TroubleTraitorWin { get; set; } = "<color=red>Traitors Win!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
    public string TroubleHumanWin { get; set; } = "<color=yellow><color=#D71868><b><i>Humans</i></b></color> Win!</color>\n<color=yellow>Elapsed Duration: <color=red>{time}</color></color>";
}