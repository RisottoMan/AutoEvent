using Exiled.API.Interfaces;

namespace AutoEvent.Games.Airstrike
{
#if EXILED
    public class DeathTranslate : ITranslation 
#else
    public class DeathTranslate
#endif
    {
        public string DeathCommandName { get; set; } = "airstrike";
        public string DeathName { get; set; } = "Airstrike Party";
        public string DeathDescription { get; set; } = "Survive as aistrikes rain down from above.";
        public string DeathCycle { get; set; } = "<color=yellow>Dodge the airstrikes!</color>\n<color=green>{time} seconds have elapsed</color>\n<color=red>{count} players left</color>";
        public string DeathMorePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow><color=red>{count}</color> players survived.</color>\n<color=#ffc0cb>{time}</color>";
        public string DeathOnePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow>Winner - <color=red>{winner}</color></color>\n<color=#ffc0cb>{time}</color>";
        public string DeathAllDie { get; set; } = "<color=red>Death Party</color>\n<color=yellow>No one survived.(((</color>\n<color=#ffc0cb>{time}</color>";
    }
}