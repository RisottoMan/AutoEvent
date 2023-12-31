using Exiled.API.Interfaces;

namespace AutoEvent.Games.CounterStrike
{
#if EXILED
    public class StrikeTranslation  : ITranslation 
#else
    public class StrikeTranslation
#endif
    {
        public string StrikeCommandName { get; set; } = "cs";
        public string StrikeSName { get; set; } = "Counter-Strike";
        public string StrikeDescription { get; set; } = "Fight between terrorists and counter-terrorists.";
        public string StrikeCycle { get; set; } = "<size=60>{name}</size>\n<size=25><i>{task}</i>\n<i><color=#42AAFF>{ctCount} Counters</color> <b>/</b> <color=green>{tCount} Terrorists</color></i>\n<i>Round Time: {time}</i></size>";
        public string StrikeNoPlantedCounter { get; set; } = "Protect plants A and B from terrorists";
        public string StrikeNoPlantedTerror { get; set; } = "Place the bomb on plant A or B";
        public string StrikePlantedCounter { get; set; } = "<color=red>Defuse the bomb before it explodes</color>";
        public string StrikePlantedTerror { get; set; } = "<color=red>Protect the bomb until it explodes</color>";
        public string StrikeDraw { get; set; } = "<b><color=#808080>Draw</color></b>\n<i>Everyone died</i>";
        public string StrikeCounterWin { get; set; } = "<b><color=#42AAFF>Counter-Terrorists win</color></b>\n<i>All the terrorists are dead</i>";
        public string StrikeTerroristWin { get; set; } = "<b><color=green>Terrorists win</color></b>\n<i>All the Counters are dead</i>";
        public string StrikePlantedWin { get; set; } = "<b><color=green>Terrorists win</color></b>\n<i>Bomb exploded</i>";
        public string StrikeDefusedWin { get; set; } = "<b><color=#42AAFF>Counter-Terrorists win</color></b>\n<i>Bomb defused</i>";
        public string StrikeYouPlanted { get; set; } = "<b><color=#ff4c5b>You planted the bomb</color></b>";
        public string StrikeYouDefused { get; set; } = "<b><color=#42aaff>You defused the bomb</color></b>";
        public string StrikeHintCycle { get; set; } = "<size=30><i><b>{name}</b>\n{kills}";
    }
}