using AutoEvent.Interfaces;

namespace AutoEvent.Games.BuckshotRoulette;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Buckshot Roulette";
    public override string Description { get; set; } = "One-on-one battle in Russian roulette with shotguns";
    public override string CommandName { get; set; } = "shotgun";
    public string PlayersNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nGo inside the arena to fight each other!\n<color=red>{remain}</color> seconds left";
    public string ClassDNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nThe player left alive <color=yellow>{scientist}</color>\nGo inside in <color=orange>{remain}</color> seconds";
    public string ScientistNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nThe player left alive <color=orange>{classd}</color>\nGo inside in <color=yellow>{remain}</color> seconds";
    public string PlayersDuel { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>";
    public string ClassDWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>CLASS D</color></color>";
    public string ScientistWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>SCIENTISTS</color></color>";
}