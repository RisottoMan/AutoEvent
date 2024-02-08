using AutoEvent.Interfaces;

namespace AutoEvent.Games.Knives;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Knives of Death";
    public override string Description { get; set; } = "Knife players against each other on a 35hp map from cs 1.6";
    public override string CommandName { get; set; } = "knives";
    public string Cycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow><color=blue>{mtfcount} MTF</color> <color=red>VS</color> <color=green>{chaoscount} CHAOS</color></color>";
    public string ChaosWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=green>CHAOS</color></color>";
    public string MtfWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>";
}