using AutoEvent.Interfaces;

namespace AutoEvent.Games.Race;
public class Translation : EventTranslation
{
    public string Cycle { get; set; } = "{name}\n<color=yellow>Get to the end platform to win!</color>\nTime Remaining: {time}";
    public string Died { get; set; } = "<color=red>You didnt finish the race</color>";
    public string PlayersSurvived { get; set; } = "<color=red>Human wins!</color>\nSurvived {count}";
    public string OneSurvived { get; set; } = "<color=red>Human wins!</color>\nWinner: {player}";
    public string NoSurvivors { get; set; } = "<color=red>Nobody survived!</color>";
}