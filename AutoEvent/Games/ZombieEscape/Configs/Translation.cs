using AutoEvent.Interfaces;

namespace AutoEvent.Games.ZombieEscape;

public class Translation : EventTranslation 
{
    public override string Name { get; set; } = "Zombie Escape";
    public override string Description { get; set; } = "Уou need to run away from zombies and escape by helicopter.";
    public override string CommandName { get; set; } = "zombie3";
    public string ZombieEscapeBeforeStart { get; set; } = "<color=#D71868><b><i>Run Forward</i></b></color>\nInfection starts in: {time}";
    public string ZombieEscapeHelicopter { get; set; } = "<color=yellow>{name}</color>\n<color=red>Need to call helicopter.</color>\nHumans left: {count}";
    public string ZombieEscapeDied { get; set; } = "Warhead detonated";
    public string ZombieEscapeZombieWin { get; set; } = "<color=red>Zombies wins!</color>\nAll humans died";
    public string ZombieEscapeHumanWin { get; set; } = "<color=#14AAF5>Humans wins!</color>\nHumans escaped";
}