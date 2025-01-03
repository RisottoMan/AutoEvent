using AutoEvent.Interfaces;

namespace AutoEvent.Games.CounterStrike;
public class Translation : EventTranslation
{
    public string Cycle { get; set; } = "<size=60>{name}</size>\n<size=25><i>{task}</i>\n<i><color=#42AAFF>{ctCount} Counters</color> <b>/</b> <color=green>{tCount} Terrorists</color></i>\n<i>Round Time: {time}</i></size>";
    public string NoPlantedCounter { get; set; } = "Protect plants A and B from terrorists";
    public string NoPlantedTerror { get; set; } = "Plant the bomb at site A or B";
    public string PlantedCounter { get; set; } = "<color=red>Defuse the bomb before it explodes</color>";
    public string PlantedTerror { get; set; } = "<color=red>Protect the bomb until it explodes</color>";
    public string Draw { get; set; } = "<b><color=#808080>Draw</color></b>\n<i>Everyone died</i>";
    public string TimeEnded { get; set; } = "<b><color=#808080>Draw</color></b>\n<i>Round time expired.</i>";
    public string CounterWin { get; set; } = "<b><color=#42AAFF>Counter-Terrorists win</color></b>\n<i>All the terrorists are dead</i>";
    public string TerroristWin { get; set; } = "<b><color=green>Terrorists win</color></b>\n<i>All the Counters are dead</i>";
    public string PlantedWin { get; set; } = "<b><color=green>Terrorists win</color></b>\n<i>Bomb exploded</i>";
    public string DefusedWin { get; set; } = "<b><color=#42AAFF>Counter-Terrorists win</color></b>\n<i>Bomb defused</i>";
    public string YouPlanted { get; set; } = "<b><color=#ff4c5b>You planted the bomb</color></b>";
    public string YouDefused { get; set; } = "<b><color=#42aaff>You defused the bomb</color></b>";
    public string HintCycle { get; set; } = "<size=30><i><b>{name}</b>\n{kills}";
}