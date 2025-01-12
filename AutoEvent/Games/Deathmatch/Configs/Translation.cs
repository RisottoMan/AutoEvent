using AutoEvent.Interfaces;

namespace AutoEvent.Games.Deathmatch;
public class Translation : EventTranslation
{
    public string Cycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>";
    public string ChaosWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=green>CHAOS</color></color>";
    public string MtfWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>";
}