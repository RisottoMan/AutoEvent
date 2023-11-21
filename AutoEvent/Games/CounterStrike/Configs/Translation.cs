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
        public string StrikeCycle { get; set; } = "<size=60>{name}</size>\n<size=25><i>{task}</i>\n" +
            "<i><color=#42AAFF>{ctCount} Counters</color> <b>/</b> <color=green>{tCount} Terrorists</color></i>\n" +
            "<i>Round Time: {time}</i></size>";
        public string StrikeNoPlantedCounter { get; set; } = "Protect plants A and B from terrorists";
        public string StrikeNoPlantedTerror { get; set; } = "Place the bomb on plant A or B";
        public string StrikePlantedCounter { get; set; } = "Defuse the bomb before it explodes";
        public string StrikePlantedTerror { get; set; } = "Protect the bomb until it explodes";
        public string StrikeDraw { get; set; } = "Draw\nEveryone died";
        public string StrikeCounterWin { get; set; } = "Counter-Terrorists win";
        public string StrikeTerroristWin { get; set; } = "Terrorists win";
        public string StrikePlantedWin { get; set; } = "Terrorists win\nBomb exploded";
        public string StrikeDefusedWin { get; set; } = "Counter-Terrorists win\nBomb defused";
    }
}