using System.ComponentModel;

namespace AutoEvent.Games.Infection
{
    public class InfectTranslate
    {
        public string ZombieName { get; set; } = "Zombie Infection";
        public string ZombieDescription { get; set; } = "Zombie mode, the purpose of which is to infect all players.";
        public string ZombieBeforeStart { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=#ABF000>There are <color=red>{time}</color> seconds left before the game starts.</color>";
        public string ZombieCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>Humans left: <color=green>{count}</color></color>\n<color=yellow>Event time <color=red>{time}</color></color>";
        public string ZombieExtraTime { get; set; } = "Extra time: {extratime}\n<color=yellow>The <b><i>Last</i></b> person left!</color>\n<color=yellow>Event time <color=red>{time}</color></color>";
        public string ZombieWin { get; set; } = "<color=red>Zombie Win!</color>\n<color=yellow>Event time <color=red>{time}</color></color>";
        public string ZombieLose { get; set; } = "<color=yellow><color=#D71868><b><i>Humans</i></b></color> Win!</color>\n<color=yellow>Event time <color=red>{time}</color></color>";
    }
}