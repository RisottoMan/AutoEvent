using AutoEvent.Interfaces;

namespace AutoEvent.Games.GunGame;
public class Translation : EventTranslation
{
    public string Cycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow>Next <color=#D71868>{gun}</color> <color=#D71868>||</color> Need <color=#D71868>{kills}</color> kills</color>\n<color=#D71868>Leader: <color=yellow>{leadnick}</color> LVL <color=yellow>{leadgun}</color></color></b>";
    public string Winner { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>The Winner of the game: <color=green>{winner}</color></color>";
}