using AutoEvent.Interfaces;

namespace AutoEvent.Games.Glass;
public class Translation : EventTranslation
{
    public string Start { get; set; } = "<size=50>Dead Jump\nJump on fragile platforms</size>\n<size=20>Alive players: {plyAlive} \nTime left: {time}</size>";
    public string Died { get; set; } = "You fallen into lava";
    public string WinSurvived { get; set; } = "<color=yellow>Human wins! Survived {plyAlive} players</color>";
    public string Winner { get; set; } = "<color=red>Dead Jump</color>\n<color=yellow>Winner: {winner}</color>";
    public string Fail { get; set; } = "<color=red>Dead Jump</color>\n<color=yellow>All players died</color>";
    public string Push { get; set; } = "<color=green>Press <color=yellow>[Alt]</color> to push the player</color>";
}