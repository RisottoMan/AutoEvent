using AutoEvent.Interfaces;

namespace AutoEvent.Games.ZombieEscape;
public class Translation : EventTranslation 
{
    public override string Name { get; set; } = "Zombie Escape";
    public override string Description { get; set; } = "Уou need to run away from zombies and call an escape helicopter.";
    public override string CommandName { get; set; } = "zombie3";
    public string Start { get; set; } = "<color=#D71868><b><i>Run Forward</i></b></color>\nInfection starts in: {time}";
    public string Helicopter { get; set; } = "<color=yellow>{name}</color>\n<color=red>Need to call helicopter.</color>\nHumans left: {count}";
    public string Died { get; set; } = "Warhead detonated";
    public string ZombieWin { get; set; } = "<color=red>Zombies wins!</color>\nAll humans died";
    public string HumanWin { get; set; } = "<color=#14AAF5>Humans wins!</color>\nHumans escaped";
}