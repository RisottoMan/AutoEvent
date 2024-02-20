using AutoEvent.Interfaces;

namespace AutoEvent.Games.BuckshotRoulette;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Buckshot Roulette";
    public override string Description { get; set; } = "One-on-one battle in Russian roulette with shotguns";
    public override string CommandName { get; set; } = "versus2";
    public string Start { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nPlayers get ready to enter the arena\nRussian Roulette will start <color=red>{time}</color> seconds";
    public string ScientistNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nScientist enter the arena in <color=yellow>{time}</color> seconds";
    public string ClassDNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nClassD enter the arena in <color=orange>{time}</color> seconds";
    public string Cycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>";
    public string ScientistWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>SCIENTISTS</color></color>";
    public string ClassDWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>CLASS D</color></color>";
    public string Draw { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>Draw</color>\n<color=red>The admin stopped the game</color>";
    public string Killer { get; set; } = "<color=yellow>Press the button in <color=red>{time}</color> seconds</color>";
    public string Target { get; set; } = "<color=yellow>Wait for the other player to make a choice</color>";
    public string Choice { get; set; } = "<color=yellow>Which one of you is going to win?)))</color>";
    public string Defeat { get; set; } = "<color=yellow>You defeated another player, now you have a shotgun</color>";
    public string Lose { get; set; } = "<color=red>You lost in Roulette</color>";
}