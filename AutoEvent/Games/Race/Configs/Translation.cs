using AutoEvent.Interfaces;

namespace AutoEvent.Games.Race;

public class RaceTranslation : EventTranslation
{
    public override string Name { get; set; } = "Race";
    public override string Description { get; set; } = "Get to the end of the map to win";
    public override string CommandName { get; set; } = "race";
    public string Cycle { get; set; } = "{name}\n<color=yellow>Pass the finishing point to win!</color>\nTime Remaining: {time}";
    public string Died { get; set; } = "You didnt finish the race.";
    public string PlayersSurvived { get; set; } = "<color=red>Human wins!</color>\nSurvived {count}";
    public string OneSurvived { get; set; } = "<color=red>Human wins!</color>\nWinner: {player}";
    public string NoSurvivors { get; set; } = "<color=red>Nobody survived!</color>";
}