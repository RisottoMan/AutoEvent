using AutoEvent.Interfaces;

namespace AutoEvent.Games.MusicalChairs;
public class Translation : EventTranslation
{
    public string Start { get; set; } = "<color=#42aaff>Get ready to run</color>\nThe game start in: <color=red>{time}</color>";
    public string Cycle { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#ffff00>{state}</color>\n<color=#008000>{count}</color> players remaining</color>";
    public string RunDontTouch { get; set; } = "Run and dont touch the platforms";
    public string StandFree { get; set; } = "Stand on a free platform";
    public string StopRunning { get; set; } = "<color=red>You stopped running</color>";
    public string TouchAhead { get; set; } = "<color=red>You touched the platforms before you were allowed to</color>";
    public string NoTime { get; set; } = "<color=red>You didnt stand on a free platform in time</color>";
    public string MorePlayers { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>The admin canceled the game</color>";
    public string Winner { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>Winner: {winner}</color>";
    public string AllDied { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>All players died</color>";
}